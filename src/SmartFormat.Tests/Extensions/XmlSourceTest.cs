﻿using System.Xml.Linq;
using NUnit.Framework;
using SmartFormat.Core;

namespace SmartFormat.Tests
{
    [TestFixture]
    public class XmlSourceTest
    {
        public const string TwoLevelXml = "<root>" +
                                      "<Person>" +
                                      "  <FirstName>Joe</FirstName>" +
                                      "  <LastName>Doe</LastName>" +
                                      "  <Phone>123-123-1234</Phone>" +
                                      "</Person>" +
                                      "<Person>" +
                                      "  <FirstName>Jack</FirstName>" +
                                      "  <LastName>Doe</LastName>" +
                                      "  <Phone>789-789-7890</Phone>" +
                                      "</Person>" +
                                      "</root>";

        private const string OneLevelXml = "<root>" +
                                           "<FirstName>Joe</FirstName>" +
                                           "<LastName>Doe</LastName>" +
                                           "<Dob>1950-05-05</Dob>" +
                                           "</root>";
        private const string OneLevelXmlWithNameSpaces = "<my:root xmlns:my='http://tempuri.org'>" +
                                                   "<my:FirstName>Joe</my:FirstName>" +
                                                   "<my:LastName>Doe</my:LastName>" +
                                                   "<my:Dob>1950-05-05</my:Dob>" +
                                                   "</my:root>";
        private const string XmlMultipleFirstNameStr = "<root>" +
                                                       "<FirstName>Joe</FirstName>" +
                                                       "<FirstName>Jack</FirstName>" +
                                                       "<LastName>Doe</LastName>" +
                                                       "<FirstName>Jim</FirstName>" +
                                                       "</root>";

        [Test]
        public void Format_SingleLevelXml_Replaced()
        {
            // arrange
            var xmlEl = XElement.Parse(OneLevelXml);
            // act
            var res = Smart.Format("Mr. {FirstName} {LastName}", xmlEl);
            // assert
            Assert.AreEqual("Mr. Joe Doe", res);
        }

        [Test]
        public void Format_SingleLevelXml_CanAccessWithIndex0()
        {
            // arrange
            var xmlEl = XElement.Parse(OneLevelXml);
            // act
            var res = Smart.Format("Mr. {FirstName.0}", xmlEl);
            // assert
            Assert.AreEqual("Mr. Joe", res);
        }

        [Test]
        public void Format_XmlWithNamespaces_IgnoringNamespace()
        {
            // arrange
            var xmlEl = XElement.Parse(OneLevelXmlWithNameSpaces);
            // act
            var res = Smart.Format("Mr. {FirstName} {LastName}", xmlEl);
            // assert
            Assert.AreEqual("Mr. Joe Doe", res);
        }

        [Test]
        public void Format_SingleLevelXml_TemplateWithCurlyBraces_Escaped()
        {
            // arrange
            var xmlEl = XElement.Parse(OneLevelXml);
            // act
            var res = Smart.Format("Mr. {{{LastName}}}", xmlEl);
            // assert
            Assert.AreEqual("Mr. {Doe}", res);
        }

        [Test]
        public void Format_MultipleElement_AccessibleByIndex()
        {
            // arrange
            var xmlEl = XElement.Parse(XmlMultipleFirstNameStr);
            // act
            var res = Smart.Format("Mr. {FirstName.1} {LastName}", xmlEl);
            // assert
            Assert.AreEqual("Mr. Jack Doe", res);
        }

        [Test]
        public void Format_MultipleElement_WithoutIndexesReturnsFirst()
        {
            // arrange
            var xmlEl = XElement.Parse(XmlMultipleFirstNameStr);
            // act
            var res = Smart.Format("Mr. {FirstName}", xmlEl);
            // assert
            Assert.AreEqual("Mr. Joe", res);
        }

        [Test]
        public void Format_MultipleElement_FormatsCount()
        {
            // arrange
            var xmlEl = XElement.Parse(XmlMultipleFirstNameStr);
            // act
            var res = Smart.Format("There{FirstName.Count: is {} Doe | are {} Does}", xmlEl);
            // assert
            Assert.AreEqual("There are 3 Does", res);
        }
        
        [Test]
        public void Format_MultipleElement_FormatsAsList()
        {
            // arrange
            var xmlEl = XElement.Parse(XmlMultipleFirstNameStr);
            // act
            var res = Smart.Format("There are{FirstName: {}|,|, and} Doe", xmlEl);
            // assert
            Assert.AreEqual("There are Joe, Jack, and Jim Doe", res);
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void Format_TwoLevelXml_InvalidSelectors_Throws()
        {
            // arrange
            var xmlEl = XElement.Parse(TwoLevelXml);
            // act
            Smart.Format("{SomethingNonExisting}{EvenMore}", xmlEl);
        }
    }
}
