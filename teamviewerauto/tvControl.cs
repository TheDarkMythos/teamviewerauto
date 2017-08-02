﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;
using System.Threading;

namespace teamviewerauto
{
    [Guid("48011AE4-59CE-4FA5-8E2C-3F0354A21AA1")]
    public partial class tvControl: UserControl,IObjectSafety
    {
        public tvControl()
        {
            InitializeComponent();
        }

        #region IObjectSafety 成员

        private const string _IID_IDispatch = "{00020400-0000-0000-C000-000000000046}";
        private const string _IID_IDispatchEx = "{a6ef9860-c720-11d0-9337-00a0c90dcaa9}";
        private const string _IID_IPersistStorage = "{0000010A-0000-0000-C000-000000000046}";
        private const string _IID_IPersistStream = "{00000109-0000-0000-C000-000000000046}";
        private const string _IID_IPersistPropertyBag = "{37D84F60-42CB-11CE-8135-00AA004BB851}";

        private const int INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001;
        private const int INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002;
        private const int S_OK = 0;
        private const int E_FAIL = unchecked((int)0x80004005);
        private const int E_NOINTERFACE = unchecked((int)0x80004002);

        private bool _fSafeForScripting = true;
        private bool _fSafeForInitializing = true;

        [SecurityCritical]
        public int GetInterfaceSafetyOptions(ref Guid riid, ref int pdwSupportedOptions, ref int pdwEnabledOptions)
        {
            int Rslt = E_FAIL;

            string strGUID = riid.ToString("B");
            pdwSupportedOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA;
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForScripting == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForInitializing == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_DATA;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }
        [SecurityCritical]
        public int SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions)
        {
            int Rslt = E_FAIL;

            string strGUID = riid.ToString("B");
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_CALLER) &&
                         (_fSafeForScripting == true))
                        Rslt = S_OK;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_DATA) &&
                         (_fSafeForInitializing == true))
                        Rslt = S_OK;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }

        #endregion
        public string Connect(string id,string password)
        {
            try
            {

                startTeamViewer();
                fillInfo(id,password);
                Thread.Sleep(1000);
                login(id,password);
                return "sucess";


            }
            catch(Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
           
        }
        /// <summary>
        /// 填入基本信息
        /// </summary>
        private void fillInfo(string id,string password)
        {
            //查找teamviewer窗口句柄
            var teamviewer = win32.FindWindow(null, "TeamViewer");
            win32.SetForegroundWindow(teamviewer);
            IntPtr hNext = IntPtr.Zero;
            //第一个对话框
            var diag1 = win32.FindWindowEx(teamviewer, hNext, "#32770", "");
            var ComboBox = win32.FindWindowEx(diag1, hNext, "ComboBox", "");
            var editId = win32.FindWindowEx(ComboBox, hNext, "Edit", "");
            var btnLink = win32.FindWindowEx(diag1, hNext, "Button", "连接到伙伴");
            //win32.SendMessage(editId, win32.WM_SETTEXT, IntPtr.Zero, "371482");
            SendKeys.SendWait(id);
            //win32.SetWindowText(editId, "371482");
            win32.SendMessage(btnLink, win32.BM_CLICK, IntPtr.Zero, null);
        }
        /// <summary>
        /// 启动teamviewer
        /// </summary>
        private void startTeamViewer()
        {
            Process.Start(new ProcessStartInfo() { FileName = "taskkill", Arguments = " / im teamviewer.exe / t / f", WindowStyle = ProcessWindowStyle.Hidden });
            Process.Start("C:\\Program Files (x86)\\TeamViewer\\TeamViewer.exe");
            IntPtr yanZhen = IntPtr.Zero;
            while (true)
            {
                yanZhen = win32.FindWindow(null, "TeamViewer");
                if (yanZhen != IntPtr.Zero)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 输入密码登录
        /// </summary>
        /// <param name="password"></param>
        private void login(string id,string password)
        {
            IntPtr hNext = IntPtr.Zero;
            IntPtr yanZhen = IntPtr.Zero;
            IntPtr btnLogin = IntPtr.Zero;
            while (true)
            {
                yanZhen = win32.FindWindow(null, "TeamViewer验证");
                if (yanZhen != IntPtr.Zero)
                {
                    SendKeys.SendWait(password);
                    btnLogin = win32.FindWindowEx(yanZhen, hNext, "Button", "登录");
                    win32.SendMessage(btnLogin, win32.BM_CLICK, IntPtr.Zero, null);
                    break;
                }
            }
        }
    }
}
