#region License

/* Copyright (c) 2006 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

#region Revisions
// 2019/11/15   JEDoub  Added an Enabled status flag that mirrors IsRunning status. I left IsRunning for backwards compatibility.
// 2019/11/18   JEDoub  Small optimizations.
// 2019/11/20   JEDoub  Enable made private. Just delete the assignment in the FORM1 if present. (mmTimer.Enabled = true; // Is Not allowed)
// 2019/11/27   JEDoub  Eliminated Warnings seen when the MultimediaTimer is used on 64-Bit OS Platforms.
//                      Made a NATIVE CLASS for the winDLLs. I also had to make the methods and variables match the native API size for 64-bit platforms.

#endregion

#region Notes
/// I found to allow the mmTimer to access Form objects the MultimediaTimer.src needs to include
/// using System.ComponentModel;
/// The public class Form1.cs or the Form1Designer.cs must also include the following statements
///         private MultimediaTimer mmTimer;

/// The public Form1() method has the following statements
///         mmTimer = new MultimediaTimer(this.components);
///         mmTimer.Stop();

////////////// Example of Setting the properties.
///         mmTimer.Period = 10;
///         mmTimer.Resolution = 1;
///         mmTimer.Mode = TimerMode.Periodic;
///         mmTimer.SynchronizingObject = this;

////////////// Hook up the Elapsed event for the timer.
///         mmTimer.Tick += new EventHandler(mmTimerTick);
///         
/// It doesn't capture the time it is invoked or the execution time of the timer like the other MultimediaTimer.
/// Use the following statement to update mm_Timer Cycle Stats
///            msPeriod = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond; 
///            OR use the built-in Kvaser CAN timer call (32-Bit) msPeriod = Canlib.canReadTimer(CANsetupForm.ESNhnd);
///            
/// TO REPLACE ALL THE multimediaTimer source files at once use the OpSys CMD as follows:
/// replace "PATHxxx\MultimediaTimer_src\MultimediaTimer.cs" "VisualStudioPATH\My Documents\Visual Studio 2017\Projects\" /s

#endregion

using System;
using System.ComponentModel;

// Used By MultiMedia Timer DLL Win_mm
using System.Runtime.InteropServices;
// Used by Debug CALL in Multimedia Timer to write a line to the console
using System.Diagnostics;


namespace Multimedia
{
    /// <summary>
    /// Defines constants for the multimedia Timer's event types.
    /// </summary>
    public enum TimerMode
    {
        /// <summary>
        /// Timer event occurs once.
        /// </summary>
        OneShot,

        /// <summary>
        /// Timer event occurs periodically.
        /// </summary>
        Periodic
    };

    /// <summary>
    /// Represents information about the multimedia Timer's capabilities.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TimerCaps
    {
        /// <summary>
        /// Minimum supported period in milliseconds.
        /// </summary>
        public uint periodMin;

        /// <summary>
        /// Maximum supported period in milliseconds.
        /// </summary>
        public uint periodMax;
    }

    /// <summary>
    /// Represents the Windows multimedia timer.
    /// </summary>
    public sealed class MultimediaTimer : IComponent
    {
        #region Timer Members

        #region Delegates

        // Represents methods that raise events.
        private delegate void EventRaiser(EventArgs e);

        #endregion


        #region Fields

        // Timer identifier.
        private UInt64 timerID;

        // Timer mode.
        private volatile TimerMode mode;

        // Period between timer events in milliseconds.
        private volatile uint period;

        // Timer resolution in milliseconds.
        private volatile uint resolution;

        // Called by Windows when a timer periodic event occurs.
        private NativeMethods.TimeProc timeProcPeriodic;

        // Called by Windows when a timer one shot event occurs.
        private NativeMethods.TimeProc timeProcOneShot;

        // Represents the method that raises the Tick event.
        private EventRaiser tickRaiser;

        // Indicates whether or not the timer has been disposed.
        private volatile bool disposed = false;

        // The ISynchronizeInvoke object to use for marshaling events.
        private ISynchronizeInvoke synchronizingObject = null;

        // Multimedia timer capabilities.
        private static TimerCaps caps;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Timer has started;
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when the Timer has stopped;
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Occurs when the time period has elapsed.
        /// </summary>
        public event EventHandler Tick;

        #endregion

        #region Construction

        /// <summary>
        /// Initialize class.
        /// </summary>
        static MultimediaTimer()
        {
            // Get multimedia timer capabilities.
            NativeMethods.timeGetDevCaps(ref caps, Marshal.SizeOf(caps));
        }

        /// <summary>
        /// Initializes a new instance of the Timer class with the specified IContainer.
        /// </summary>
        /// <param name="container">
        /// The IContainer to which the Timer will add itself.
        /// </param>
        public MultimediaTimer(IContainer container)
        {
            ///
            /// Required for Windows.Forms Class Composition Designer support
            ///
            container.Add(this);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Timer class.
        /// </summary>
        public MultimediaTimer()
        {
            Initialize();
        }

        ~MultimediaTimer()
        {
            if (IsRunning || Enabled)
            {
                // Stop and destroy timer.
                NativeMethods.timeKillEvent((UInt32)timerID);
            }
        }

        // Initialize timer with default values.
        private void Initialize()
        {
            this.mode = TimerMode.Periodic;
            this.period = Capabilities.periodMin;
            this.resolution = 1;

            Enabled = false;
            IsRunning = false;

            timeProcPeriodic = new NativeMethods.TimeProc(TimerPeriodicEventCallback);
            timeProcOneShot = new NativeMethods.TimeProc(TimerOneShotEventCallback);
            tickRaiser = new EventRaiser(OnTick);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The timer has already been disposed.
        /// </exception>
        /// <exception cref="TimerStartException">
        /// The timer failed to start.
        /// </exception>
        public void Start()
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("MultimediaTimer");
            }

            #endregion

            #region Guard

            if (IsRunning || Enabled)
            {
                return;
            }

            #endregion

            // If the periodic event callback should be used.
            if (Mode == TimerMode.Periodic)
            {
                // Create and start timer.
                timerID = NativeMethods.timeSetEvent(Period, Resolution, timeProcPeriodic, UIntPtr.Zero, (uint)Mode);
            }
            // Else the one shot event callback should be used.
            else
            {
                // Create and start timer.
                timerID = NativeMethods.timeSetEvent(Period, Resolution, timeProcOneShot, UIntPtr.Zero, (uint)Mode);
            }

            // If the timer was created successfully.
            if (timerID != 0)
            {
                IsRunning = true;
                Enabled = true;

                if (SynchronizingObject != null && SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(
                        new EventRaiser(OnStarted),
                        new object[] { EventArgs.Empty });
                }
                else
                {
                    OnStarted(EventArgs.Empty);
                }
            }
            else
            {
                throw new TimerStartException("Unable to start multimedia Timer.");
            }
        }

        /// <summary>
        /// Stops timer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>
        public void Stop()
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("MultimediaTimer");
            }

            #endregion

            #region Guard

            if (!IsRunning)
            {
                return;
            }

            #endregion

            // Stop and destroy timer.
            UInt32 result = NativeMethods.timeKillEvent((UInt32)timerID);

            Debug.Assert(result == NativeMethods.TIMERR_NOERROR);

            IsRunning = false;
            Enabled = false;

            if (SynchronizingObject != null && SynchronizingObject.InvokeRequired)
            {
                SynchronizingObject.BeginInvoke(
                    new EventRaiser(OnStopped),
                    new object[] { EventArgs.Empty });
            }
            else
            {
                OnStopped(EventArgs.Empty);
            }
        }

        #region Callbacks

        // Callback method called by the Win32 multimedia timer when a timer
        // periodic event occurs.
        private void TimerPeriodicEventCallback(uint id, uint msg, UIntPtr user, UIntPtr param1, UIntPtr param2)
        {
            if (synchronizingObject != null)
            {
                synchronizingObject.BeginInvoke(tickRaiser, new object[] { EventArgs.Empty });
            }
            else
            {
                OnTick(EventArgs.Empty);
            }
        }

        // Callback method called by the Win32 multimedia timer when a timer
        // one shot event occurs.
        private void TimerOneShotEventCallback(uint id, uint msg, UIntPtr user, UIntPtr param1, UIntPtr param2)
        {
            if (synchronizingObject != null)
            {
                synchronizingObject.BeginInvoke(tickRaiser, new object[] { EventArgs.Empty });
                Stop();
            }
            else
            {
                OnTick(EventArgs.Empty);
                Stop();
            }
        }

        #endregion

        #region Event Raiser Methods

        // Raises the Disposed event.
        private void OnDisposed(EventArgs e)
        {
            Disposed?.Invoke(this, e);
        }

        // Raises the Started event.
        private void OnStarted(EventArgs e)
        {
            Started?.Invoke(this, e);
        }

        // Raises the Stopped event.
        private void OnStopped(EventArgs e)
        {
            Stopped?.Invoke(this, e);
        }

        // Raises the Tick event.
        private void OnTick(EventArgs e)
        {
            Tick?.Invoke(this, e);
        }

        #endregion        

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls.
        /// </summary>
        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                #region Require

                if (disposed)
                {
                    throw new ObjectDisposedException("MultimediaTimer");
                }

                #endregion

                return synchronizingObject;
            }
            set
            {
                #region Require

                if (disposed)
                {
                    throw new ObjectDisposedException("MultimediaTimer");
                }

                #endregion

                synchronizingObject = value;
            }
        }

        /// <summary>
        /// Gets or sets the time between Tick events.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>   
        public uint Period
        {
            get
            {
                #region Require

                if (disposed)
                {
                    throw new ObjectDisposedException("MultimediaTimer");
                }

                #endregion

                return period;
            }
            set
            {
                #region Require

                if (disposed)
                {
                    throw new ObjectDisposedException("MultimediaTimer");
                }
                else if (value < Capabilities.periodMin || value > Capabilities.periodMax)
                {
                    throw new ArgumentOutOfRangeException("Period", value,
                        "Multimedia Timer period out of range.");
                }

                #endregion

                period = value;

                if (IsRunning || Enabled)
                {
                    Stop();
                    Start();
                }
            }
        }

        /// <summary>
        /// Gets or sets the timer resolution.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>        
        /// <remarks>
        /// The resolution is in milliseconds. The resolution increases 
        /// with smaller values; a resolution of 0 indicates periodic events 
        /// should occur with the greatest possible accuracy. To reduce system 
        /// overhead, however, you should use the maximum value appropriate 
        /// for your application.
        /// </remarks>
        public uint Resolution
        {
            get
            {
                #region Require

                if (disposed)
                {
                    throw new ObjectDisposedException("MultimediaTimer");
                }

                #endregion

                return resolution;
            }
            set
            {
                #region Require

                if (disposed)
                {
                    throw new ObjectDisposedException("MultimediaTimer");
                }
                else if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Resolution", value,
                        "Multimedia timer resolution out of range.");
                }

                #endregion

                resolution = value;

                if (IsRunning || Enabled)
                {
                    Stop();
                    Start();
                }
            }
        }

        /// <summary>
        /// Gets the timer mode.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>
        public TimerMode Mode
        {
            get
            {
                #region Require

                if (disposed)
                {
                    throw new ObjectDisposedException("MultimediaTimer");
                }

                #endregion

                return mode;
            }
            set
            {
                #region Require

                if (disposed)
                {
                    throw new ObjectDisposedException("MultimediaTimer");
                }

                #endregion

                mode = value;

                if (IsRunning || Enabled)
                {
                    Stop();
                    Start();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Timer is running.
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        /// <summary> 
        /// Gets or sets a value indicating whether the  
        /// System.Timers.MultimediaTimer should raise 
        /// the System.Timers.MultimediaTimer.Elapsed event. 
        /// 
        /// Returns: 
        ///     true if the System.Timers.MultimediaTimer should raise the  
        ///     System.Timers.MultimediaTimer.Elapsed 
        ///     event; otherwise, false. The default is false. 
        ///         
        /// </summary> 
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value> 
        public bool Enabled { get; private set; } = false;

        /// <summary>
        /// Gets the timer capabilities.
        /// </summary>
        public static TimerCaps Capabilities
        {
            get
            {
                return caps;
            }
        }

        #endregion

        #endregion

        #region IComponent Members

        public event System.EventHandler Disposed;

        public ISite Site { get; set; } = null;

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Frees timer resources.
        /// </summary>
        public void Dispose()
        {
            #region Guard

            if (disposed)
            {
                return;
            }

            #endregion               

            if (IsRunning || Enabled)
            {
                Stop();
            }

            disposed = true;

            OnDisposed(EventArgs.Empty);
        }

        #endregion       
    }

    /// <summary>
    /// The exception that is thrown when a timer fails to start.
    /// </summary>
    [Serializable()]
    public class TimerStartException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the TimerStartException class.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        public TimerStartException(string message) : base(message)
        {
        }
    }

    internal static class NativeMethods
    {
        // Represents the method that is called by Windows when a timer event occurs.
        public delegate void TimeProc(uint id, uint msg, UIntPtr user, UIntPtr param1, UIntPtr param2);

        #region Win32 Multimedia Timer Functions

        // Gets timer capabilities.
        [DllImport("winmm.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 timeGetDevCaps(ref TimerCaps caps,
            int sizeOfTimerCaps);

        // Creates and starts the timer.
        [DllImport("winmm.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 timeSetEvent(uint delay, uint resolution,
            TimeProc proc, UIntPtr user, uint mode);

        // Stops and destroys the timer.
        [DllImport("winmm.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 timeKillEvent(UInt32 id);

        // Indicates that the operation was successful.
        public const uint TIMERR_NOERROR = 0;

        #endregion

    }
}