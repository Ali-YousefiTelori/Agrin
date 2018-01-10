namespace Agrin.UI.Helper.SingleApplication
{
    #region Namespaces
    using System;

    #endregion

    /// <summary>
    ///   Class used to define the arguments of another application instance startup.
    /// </summary>
    public class StartupNextInstanceEventArgs : EventArgs
    {
        #region Fields
        /// <summary>
        ///   The application arguments.
        /// </summary>
        private readonly string[] m_Args;
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets a value indicating whether the application main window has to be brought to foreground.
        /// </summary>
        /// <value><c>True</c> if the application windiow has to be brought to foreground, otherwise <c>false</c>.</value>
        public bool BringToForeground { get; set; }

        /// <summary>
        ///   Gets the arguments passed to the other application.
        /// </summary>
        /// <value>The arguments passed to the other application.</value>
        public string[] Args
        {
            get { return m_Args; }
        }
        #endregion

        /// <summary>
        ///   Initializes a new instance of the <see cref = "StartupNextInstanceEventArgs" /> class.
        /// </summary>
        /// <param name = "args">The args.</param>
        public StartupNextInstanceEventArgs(string[] args)
                : this(args, true)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "StartupNextInstanceEventArgs" /> class.
        /// </summary>
        /// <param name = "args">The args.</param>
        /// <param name = "bringToFront">If set to <c>true</c> the application main window will be brought to front.</param>
        public StartupNextInstanceEventArgs(string[] args, bool bringToFront)
        {
            if (args == null)
                args = new string[0];

            m_Args = args;
            BringToForeground = bringToFront;
        }
    }
}