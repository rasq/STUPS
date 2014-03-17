﻿/*
 * Created by SharpDevelop.
 * User: Alexander Petrovskiy
 * Date: 3/15/2014
 * Time: 11:20 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace PSTestLib
{
    using System;
    using System.Linq;
    using Ninject;
    using Ninject.Modules;
    
    using Ninject.Parameters;
    using Ninject.Extensions.ChildKernel;
    using System.Collections;
//    using System.Collections.Generic;
    using Castle.DynamicProxy;
    
    using NLog;
    
    /// <summary>
    /// Description of ObjectFactory.
    /// </summary>
    // public class ObjectFactory
    public static class ObjectFactory
    {
        private static IKernel _kernel { get; set; }
        private static ProxyGenerator _generator;
        private static bool _alreadyInitialized;
        
        // public ObjectFactory()
        static ObjectFactory()
        {
//Console.WriteLine("OF 01");
            if (_alreadyInitialized) return;
Console.WriteLine("OF 02");
            Init(new CommonModule());
//Console.WriteLine("OF 03");
            _alreadyInitialized = true;
        }
        
        public static void Init(params INinjectModule[] modules)
        {
////Console.WriteLine("OF.Init 01");
//            if (null != _kernel) {
////Console.WriteLine("OF.Init 02");
//                var currentModules = _kernel.GetModules();
////Console.WriteLine("Modules number == {0}", currentModules.Count().ToString());
////Console.WriteLine("OF.Init 03");
//                if (null != currentModules && currentModules.Any()) {
////Console.WriteLine("OF.Init 04");
//                    // modules.Union(currentModules);
//                    modules = modules.Union(currentModules).ToArray();
////Console.WriteLine("Modules to load == {0}", modules.Count().ToString());
////Console.WriteLine("OF.Init 05");
//                }
//            }
////Console.WriteLine("OF.Init 06");
//            _kernel = new StandardKernel(modules);
            
            
//Console.WriteLine("OF.Init 01");
            if (null != _kernel) {
//Console.WriteLine("OF.Init 02");
                var currentModules = _kernel.GetModules();
//Console.WriteLine("Modules number == {0}", currentModules.Count().ToString());
//Console.WriteLine("OF.Init 03");
                if (null != currentModules && currentModules.Any()) {
//Console.WriteLine("OF.Init 04");
                    // modules.Union(currentModules);
                    var allModules = modules.Union(currentModules).ToArray();
//Console.WriteLine("Modules to load == {0}", modules.Count().ToString());
//Console.WriteLine("OF.Init 05");
                    _kernel = new StandardKernel(allModules);
                } else {
                    _kernel = new StandardKernel(modules);
                }
            } else {
//Console.WriteLine("OF.Init 06");
                _kernel = new StandardKernel(modules);
            }
            
            // ??
            _kernel.Settings.ActivationCacheDisabled = false;
            
//Console.WriteLine("OF.Init 08");
            InitCommonObjects();
foreach (var module in _kernel.GetModules()) {
    Console.WriteLine(module.Name);
}
        }
        
		public static void InitCommonObjects()
		{
            // if (_alreadyInitialized) return;
		    var argument = new ConstructorArgument("builder", new PersistentProxyBuilder());
		    _generator = _kernel.Get<ProxyGenerator>(argument);
		    
            // var logger = GetLogHelper(string.Empty);
		}
        
		public static void Reset()
		{
		    _generator = null;
		    try {
                _kernel.Dispose();
		    }
		    catch {}
		    try {
                _kernel = null;
		    }
		    catch {}
//		    _ninjectModule = null;
//		    _initFlag = false;
            _alreadyInitialized = false;
		}
        
        public static T Resolve<T>(params IParameter[] parameters)
        {
//Console.WriteLine("Resolve 001");
//if (null == _kernel) {
//    Console.WriteLine("null == _kernel");
//} else {
//    Console.WriteLine("null != _kernel");
//}
//Console.WriteLine("0000000000000001++");
            return _kernel.Get<T>(parameters);
        }
        
        public static void Release(object objectToRelease)
        {
            _kernel.Release(objectToRelease);
        }
        
		#region Castle DynamicProxy
        public static T ConvertToProxiedObject<T>(T classToProxy, IInterceptor[] interceptors, params Type[] additionalInterfaces)
        // public static T ConvertToProxiedObject<T>(IInterceptor[] interceptors, params Type[] additionalInterfaces)
        {
            
            T proxiedObject = default(T);
            
            try {
                
//Console.WriteLine("ConvertToProxiedObject: 000001");
                if (null != additionalInterfaces && 0 < additionalInterfaces.Length) {
Console.WriteLine("ConvertToProxiedObject: 000002");
Console.WriteLine(typeof(T).Name);
            		proxiedObject =
            		    (T)_generator.CreateClassProxy(
            		        typeof(T),
            		        additionalInterfaces,
                            interceptors);
Console.WriteLine("ConvertToProxiedObject: 000003");
                } else {
//Console.WriteLine("ConvertToProxiedObject: 000004");
            		proxiedObject =
            		    (T)_generator.CreateClassProxy(
            		        typeof(T),
                            interceptors);
//Console.WriteLine("ConvertToProxiedObject: 000005");
                }
            }
            catch (Exception eConvertation) {
//Console.WriteLine("ConvertToProxiedObject: 000006");
Console.WriteLine("ConvertToProxiedObject<T>001: 005");
                Console.WriteLine(eConvertation.Message);
            }
//Console.WriteLine("ConvertToProxiedObject: 000007");
    		return proxiedObject;
        }
        
        public static T ConvertToProxiedObject<T>(T classToProxy, PSCmdletBase cmdlet, IInterceptor[] interceptors) //, params Type[] additionalInterfaces)
        // public static T ConvertToProxiedObject<T>(IInterceptor[] interceptors, params Type[] additionalInterfaces)
        {
            
            T proxiedObject = default(T);
            
            
            //
            //
            // temporary!
            // interceptors = new IInterceptor[] { new EmptyAspect() };
            //
            //
            
            try {
                
//Console.WriteLine("ConvertToProxiedObject: 000001+");
//Console.WriteLine("cmdlet is {0}", cmdlet.GetType().Name);
//Console.WriteLine("type is {0}", typeof(T).Name);
//foreach (var icept in interceptors) {
//    Console.WriteLine(icept.GetType().Name);
//}
        		proxiedObject =
        		    (T)_generator.CreateClassProxy(
        		        typeof(T),
        		        new PSCmdletBase[]{ cmdlet },
        		        // new[]{ cmdlet },
                        interceptors);
//Console.WriteLine("ConvertToProxiedObject: 000002");
            }
            catch (Exception eConvertation) {
//Console.WriteLine("ConvertToProxiedObject: 000003");
Console.WriteLine("ConvertToProxiedObject<T>002: 005");
                Console.WriteLine(eConvertation.Message);
            }
//Console.WriteLine("ConvertToProxiedObject: 000004");
    		return proxiedObject;
        }
        
        public static T ConvertToProxiedObject<T>(Type classToProxy, IInterceptor[] interceptors, params Type[] additionalInterfaces)
        // public static T ConvertToProxiedObject<T>(IInterceptor[] interceptors, params Type[] additionalInterfaces)
        {
            
            T proxiedObject = default(T);
            
            try {
                
// Console.WriteLine("ConvertToProxiedObject: 000001");
                if (null != additionalInterfaces && 0 < additionalInterfaces.Length) {
//Console.WriteLine("ConvertToProxiedObject: 000002");
//Console.WriteLine(typeof(T).Name);
            		proxiedObject =
            		    (T)_generator.CreateClassProxy(
            		        // typeof(N),
                            classToProxy,
            		        additionalInterfaces,
                            interceptors);
//Console.WriteLine("ConvertToProxiedObject: 000003");
                } else {
//Console.WriteLine("ConvertToProxiedObject: 000004");
            		proxiedObject =
            		    (T)_generator.CreateClassProxy(
            		        // typeof(N),
                            classToProxy,
                            interceptors);
//Console.WriteLine("ConvertToProxiedObject: 000005");
                }
            }
            catch (Exception eConvertation) {
//Console.WriteLine("ConvertToProxiedObject: 000006");
Console.WriteLine("ConvertToProxiedObject<T>003: 005");
                Console.WriteLine(eConvertation.Message);
            }
//Console.WriteLine("ConvertToProxiedObject: 000007");
    		return proxiedObject;
        }
        #endregion Castle DynamicProxy
        
        public static LogHelper GetLogHelper(string logPath)
        {
            try {
                
                var logger = _kernel.Get<LogHelper>();
                if (!string.IsNullOrEmpty(logPath)) {
                    logger.LogPath = logPath;
                }
                return logger;
                
            } catch (Exception eLogger) {
                // TODO
                // write error to error object!!!
			    // Console.WriteLine("PSTestLib.LogHelper 01");
			    // Console.WriteLine(eLogger.Message);
                return null;
            }
        }
        
        public static Logger GetLogger(string logPath)
        {
            var logHelper = GetLogHelper(logPath);
            // 20140304
            return logHelper.UiaLogger;
            // return ConvertToProxiedObject<Logger>(logHelper.UiaLogger);
        }
    }
}