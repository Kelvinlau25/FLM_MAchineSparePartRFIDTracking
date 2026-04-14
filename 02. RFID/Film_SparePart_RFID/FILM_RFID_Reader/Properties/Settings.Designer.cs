namespace FILM_RFID_READER_DEPLOY.Properties
{
    [global::System.Runtime.CompilerServices.CompilerGenerated()]
    [global::System.CodeDom.Compiler.GeneratedCode("Manual", "1.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        public static Settings Default => defaultInstance;

        [global::System.Configuration.ApplicationScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("")]
        public string READER_NOTIFICATION_MAILCC => ((string)(this["READER_NOTIFICATION_MAILCC"]));

        [global::System.Configuration.ApplicationScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("")]
        public string READER_NOTIFICATION_MAILBCC => ((string)(this["READER_NOTIFICATION_MAILBCC"]));

        [global::System.Configuration.ApplicationScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("Film")]
        public string COMPANY => ((string)(this["COMPANY"]));

        [global::System.Configuration.ApplicationScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("rosmieza@maxsys.com.my")]
        public string READER_NOTIFICATION_MAILTO => ((string)(this["READER_NOTIFICATION_MAILTO"]));

        [global::System.Configuration.ApplicationScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("False")]
        public bool TEST_ENVIRONMENT => ((bool)(this["TEST_ENVIRONMENT"]));
    }
}
