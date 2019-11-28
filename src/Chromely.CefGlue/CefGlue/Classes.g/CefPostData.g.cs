﻿//
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
//
#pragma warning disable 1591
namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    // Role: PROXY
    public sealed unsafe partial class CefPostData : IDisposable
    {
        internal static CefPostData FromNative(cef_post_data_t* ptr)
        {
            return new CefPostData(ptr);
        }
        
        internal static CefPostData FromNativeOrNull(cef_post_data_t* ptr)
        {
            if (ptr == null) return null;
            return new CefPostData(ptr);
        }
        
        private cef_post_data_t* _self;
        
        private CefPostData(cef_post_data_t* ptr)
        {
            if (ptr == null) throw new ArgumentNullException("ptr");
            _self = ptr;
        }
        
        ~CefPostData()
        {
            if (_self != null)
            {
                Release();
                _self = null;
            }
        }
        
        public void Dispose()
        {
            if (_self != null)
            {
                Release();
                _self = null;
            }
            GC.SuppressFinalize(this);
        }
        
        internal void AddRef()
        {
            cef_post_data_t.add_ref(_self);
        }
        
        internal bool Release()
        {
            return cef_post_data_t.release(_self) != 0;
        }
        
        internal bool HasOneRef
        {
            get { return cef_post_data_t.has_one_ref(_self) != 0; }
        }
        
        internal bool HasAtLeastOneRef
        {
            get { return cef_post_data_t.has_at_least_one_ref(_self) != 0; }
        }
        
        internal cef_post_data_t* ToNative()
        {
            AddRef();
            return _self;
        }
    }
}
