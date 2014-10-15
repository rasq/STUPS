﻿/*
 * Created by SharpDevelop.
 * User: Alexander Petrovskiy
 * Date: 10/3/2014
 * Time: 5:05 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace Tmx.Server.Modules
{
    using System;
    using Nancy;
    using Tmx.Interfaces.Server;
    
    /// <summary>
    /// Description of RootPageModule.
    /// </summary>
    public class RootPageModule : NancyModule
    {
        public RootPageModule()
        {
            Get[UrnList.RootPage_Root] = _ => View[UrnList.RootPage_RootPageName];
            Get[UrnList.RootPage_ScriptsFolder] = _ => null;
        }
    }
}