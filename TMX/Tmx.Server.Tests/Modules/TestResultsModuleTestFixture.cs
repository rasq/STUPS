﻿/*
 * Created by SharpDevelop.
 * User: Alexander Petrovskiy
 * Date: 7/13/2014
 * Time: 2:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace Tmx.Server.Tests.Modules
{
    using System;
	using System.Collections.Generic;
	using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
	using System.Xml.Linq;
	using NSubstitute;
    using Nancy;
    using Nancy.Json;
    using Nancy.Testing;
    using Tmx.Core;
    using Tmx.Core.Types.Remoting;
    using Tmx.Interfaces.Remoting;
	using Tmx.Interfaces.Server;
	using Tmx.Server.Modules;
    // using MbUnit.Framework;
    using NUnit.Framework;
	using Tmx;
	using Tmx.Interfaces.TestStructure;
    using Xunit;
    using Tmx.Interfaces;
    using PSTestLib;
    
    /// <summary>
    /// Description of TestResultsModuleTestFixture.
    /// </summary>
    [MbUnit.Framework.TestFixture][NUnit.Framework.TestFixture]
    public class TestResultsModuleTestFixture
    {
	    ITestWorkflow _workflow;
	    ITestRun _testRun;
	    BrowserResponse _response;
	    Browser _browser;
        
    	public TestResultsModuleTestFixture()
    	{
    	    TestSettings.PrepareModuleTests();
		    _browser = TestFactory.GetBrowserForTestResultsModule();
		    TestFactory.GetTestRunWithStatus(TestRunStatuses.Running);
		    _workflow = WorkflowCollection.Workflows.First();
		    _testRun = TestRunQueue.TestRuns.First();
//		    JsonSettings.MaxJsonLength = 10000000;
//		    Nancy.Json.JsonSettings.MaxJsonLength = 10000000;
//		    Nancy.Json.JsonSettings.MaxRecursions = 25;
    	}
		
    	[MbUnit.Framework.SetUp][NUnit.Framework.SetUp]
    	public void SetUp()
    	{
    	    TestSettings.PrepareModuleTests();
		    _browser = TestFactory.GetBrowserForTestResultsModule();
		    TestFactory.GetTestRunWithStatus(TestRunStatuses.Running);
		    _workflow = WorkflowCollection.Workflows.First();
		    _testRun = TestRunQueue.TestRuns.First();
//		    JsonSettings.MaxJsonLength = 10000000;
//		    Nancy.Json.JsonSettings.MaxJsonLength = 10000000;
//		    Nancy.Json.JsonSettings.MaxRecursions = 25;
    	}
    	
//    	[MbUnit.Framework.Test][NUnit.Framework.Test][Fact]
//    	public void Should_ignore_empty_results_collection()
//    	{
//    	    Xunit.Assert.Equal(0, 1);
//    	}
//    	
//    	[MbUnit.Framework.Test][NUnit.Framework.Test][Fact]
//    	public void Should_add_test_suites_from_results_collection()
//    	{
//    	    Xunit.Assert.Equal(0, 1);
//    	}
//    	
//    	[MbUnit.Framework.Test][NUnit.Framework.Test][Fact]
//    	public void Should_add_test_suites_and_test_scenarios_from_results_collection()
//    	{
//    	    Xunit.Assert.Equal(0, 1);
//    	}
//    	
//    	[MbUnit.Framework.Test][NUnit.Framework.Test][Fact]
//    	public void Should_add_test_results_from_results_collection()
//    	{
//    	    Xunit.Assert.Equal(0, 1);
//    	}
    	
        [MbUnit.Framework.Test][NUnit.Framework.Test][Fact]
        public void Should_react_on_posting_no_data()
        {
            var dataObject = new TestResultsDataObject {
                Data = string.Empty
            };
            
            WHEN_Posting_TestResults<TestResultsDataObject>(dataObject);
            
            THEN_HttpResponse_Is_Created();
        }
        
        [MbUnit.Framework.Test][NUnit.Framework.Test][Fact]
        public void Should_react_on_posting_bunch_of_data()
        {
        	var xDoc = XDocument.Load(@"../../Modules/TMX_report.xml");
//        	var xDoc = XDocument.Load(@"../../Modules/TMX_red_report.xml");
            
            var dataObject = new TestResultsDataObject {
                Data = xDoc.ToString()
            };
            
            WHEN_Posting_TestResults<TestResultsDataObject>(dataObject);
            
            THEN_HttpResponse_Is_Created();
        }
        
        [MbUnit.Framework.Test][NUnit.Framework.Test][Fact]
        public void Should_send_one_test_suite_with_inner_data()
        {
            var suites = GIVEN_one_testSuite_with_inner_hierarchy();
            var testResultsExporter = new TestResultsImportExport();
            var xDoc = testResultsExporter.GetTestResultsAsXdocument(new SearchCmdletBaseDataObject {
                                                                         FilterAll = true,
                                                                         OrderById = true
                                                                     },
                                                                     suites);
            var dataObject = new TestResultsDataObject {
                Data = xDoc.ToString()
            };
            
            WHEN_Posting_TestResults<TestResultsDataObject>(dataObject);
            
            THEN_HttpResponse_Is_Created();
            Xunit.Assert.Equal(suites[0].Id, _testRun.TestSuites[0].Id);
            Xunit.Assert.Equal(suites[0].TestScenarios[0].Id, _testRun.TestSuites[0].TestScenarios[0].Id);
            Xunit.Assert.Equal(suites[0].TestScenarios[0].TestResults[0].Id, _testRun.TestSuites[0].TestScenarios[0].TestResults[0].Id);
        }
        
        [MbUnit.Framework.Test][NUnit.Framework.Test][Fact]
        public void Should_receive_test_results_from_test_run()
        {
            var suites = GIVEN_one_testSuite_with_inner_hierarchy();
            _testRun.TestSuites.AddRange(suites);
            
            WHEN_Getting_TestResults();
            
            // var testResultsImporter = new TestResultsImportExport();
            // var result = testResultsImporter.ImportTestResultsFromXdocument(_response.Body as XDocument);
            
            Console.WriteLine(_response.Body);
            Console.WriteLine(_response.Body.GetType().Name);
//            var loadedSuites = _response..Body as ITestSuite[];
//            Xunit.Assert.Equal(suites[0].Id, loadedSuites[0].Id);
        }
        
        // ============================================================================================================================
        void probe()
        {
            var suite01 = Substitute.For<ITestSuite>();
            suite01.Id = "1";
            suite01.Name = "s01";
            var suite02 = Substitute.For<ITestSuite>();
            suite02.Id = "2";
            suite02.Name = "s02";
            
        }
        
        XElement getElementWithTestResults(IOrderedEnumerable<ITestSuite> suites, IOrderedEnumerable<ITestScenario> scenarios, IOrderedEnumerable<ITestResult> testResults)
        {
            return TmxHelper.CreateSuitesXElementWithParameters(suites, scenarios, testResults, (new XMLElementsNativeStruct()));            
        }
        
        XElement GIVEN_empty_element()
        {
            return new XElement("suites");
        }
        
        XElement GIVEN_suite_element()
        {
            var suites = new XElement("suites");
            suites.AddFirst(new XElement("suite"));
            return suites;
        }
        
        List<ITestSuite> GIVEN_one_testSuite_with_inner_hierarchy()
        {
            var suites = new List<ITestSuite>() {
                new TestSuite {
                    Id = "1",
                    Name = "s01",
                    PlatformId = "3"
                }
            };
            var testScenario = new TestScenario {
                Id = "2",
                Name = "sc01",
                PlatformId = "3",
                SuiteId = "1"
            };
            testScenario.TestResults.Add(new TestResult {
                Id = "4",
                Name = "tr01",
                PlatformId = "3",
                SuiteId = "1",
                ScenarioId = "2",
                Origin = TestResultOrigins.Logical,
                enStatus = TestResultStatuses.Passed
            });
            suites[0].TestScenarios.Add(testScenario);
            return suites;
        }
        
//        XElement GIVEN_one_test_suite()
//        {
//            return new XElement("aaa");
//        }
//        
//        XElement GIVEN_three_test_suites()
//        {
//            return new XElement("aaa");
//        }
//        
//        XElement GIVEN_one_test_suite_and_one_test_scenario()
//        {
//            return new XElement("aaa");
//        }
//        
//        XElement GIVEN_one_test_suite_and_three_test_scenarios()
//        {
//            return new XElement("aaa");
//        }
//        
//        XElement GIVEN_one_test_suite_and_one_test_scenario_and_one_test_result()
//        {
//            return new XElement("aaa");
//        }
        
        TestSuite GIVEN_test_suite(string testSuiteId, string testSuiteName)
        {
            var testSuite = Substitute.For<TestSuite>();
            testSuite.Name = testSuiteName;
            testSuite.Id = testSuiteId;
            testSuite.PlatformId = TestData.GetDefaultPlatformId();
            return testSuite;
        }
        
        TestScenario GIVEN_test_scenario(string testScenarioId, string testScenarioName, string testSuiteId, string testPlatformId)
        {
            var testScenario = Substitute.For<TestScenario>();
            testScenario.Name = testScenarioName;
            testScenario.Id = testScenarioId;
            testScenario.PlatformId = testPlatformId;
            testScenario.SuiteId = testSuiteId;
            return testScenario;
        }
        
        TestResult GIVEN_test_result(string testResultName, TestResultStatuses status)
        {
            var testResult = new TestResult();
            testResult.Name = testResultName;
            testResult.enStatus = status;
            return testResult;
        }
        
        void WHEN_Posting_TestResults<T>(T element)
        {
			_response = _browser.Post(getPathToResourcesCollection(typeof(T)), (with) => {
			                              with.JsonBody<T>(element);
			                              with.Accept("application/json");
			                          });
        }
        
        void WHEN_Getting_TestResults()
        {
			// _response = _browser.Get(getPathToResourcesCollection(typeof(XDocument)), (with) => {
			_response = _browser.Get(getPathToResourcesCollection(typeof(List<ITestSuite>)), (with) => {
			                              with.Accept("application/json");
			                          });
        }
        
        string getPathToResourcesCollection(MemberInfo type)
        {
            string path = string.Empty;
Console.WriteLine("test run id = {0}", _testRun.Id);
Console.WriteLine("type name = {0}", type.Name);
			switch (type.Name) {
			    case "XElement":
                case "XDocument":
    case ".XDocument":
    case "String":
			        // return UrnList.TestResultsPostingPoint_absPath;
var aaa = UrnList.TestResults_Root + "/" + _testRun.Id + UrnList.TestResultsPostingPoint_forClient_relPath;
Console.WriteLine("url = {0}", aaa);
			        return UrnList.TestResults_Root + "/" + _testRun.Id + UrnList.TestResultsPostingPoint_forClient_relPath;
            // 20141031
            // postponed
//			    case "TestSuite":
//			        return UrnList.TestStructure_Root + UrnList.TestStructure_Suites;
//			    case "TestScenario":
//			        return UrnList.TestStructure_Root + UrnList.TestStructure_Scenarios;
//			    case "TestResult":
//			        return UrnList.TestStructure_Root + UrnList.TestStructure_Results;
			    default:
			        // return path;
			        return UrnList.TestResults_Root + "/" + _testRun.Id + UrnList.TestResultsPostingPoint_forClient_relPath;
			}
        }
        
//        ITestClient givenSendingRegistration(ITestClient testClient)
//        {
//            var response = whenSendingRegistration(testClient);
//            return response.Body.DeserializeJson<TestClient>();
//        }
//        
//        BrowserResponse whenSendingRegistration(ITestClient testClient)
//        {
//            var browser = TestFactory.GetBrowserForTestResultsModule();
//            return browser.Post(UrnList.TestClientRegistrationPoint, with => with.JsonBody<ITestClient>(testClient));
//        }
//        
//        void whenSendingDeregistration(ITestClient testClient)
//        {
//            var browser = TestFactory.GetBrowserForTestResultsModule();
//            browser.Delete(UrnList.TestClients_Root + "/" + testClient.Id);
//        }
        
        // void THEN_HttpResponse_Is_Created(BrowserResponse response)
        void THEN_HttpResponse_Is_Created()
        {
            Xunit.Assert.Equal(HttpStatusCode.Created, _response.StatusCode);
        }
        
        // void THEN_HttpResponse_Is_ExpectationFailed(BrowserResponse response)
        void THEN_HttpResponse_Is_ExpectationFailed()
        {
            Xunit.Assert.Equal(HttpStatusCode.ExpectationFailed, _response.StatusCode);
        }
    }
}
