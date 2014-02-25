using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.InformationManager.Common.Domain;

namespace Test.InformationManager.Common.Domain
{
    [TestClass]
    public class EmailValidationAttributeTest
    {
        [TestMethod]
        public void ValidateEmail()
        {
            var emailValidationAttribute = new EmailValidationAttribute();

            Assert.IsTrue(emailValidationAttribute.IsValid("jesper.aaberg@example.com"));
            Assert.IsTrue(emailValidationAttribute.IsValid("jesper.aaberg@example.de"));

            Assert.IsFalse(emailValidationAttribute.IsValid("asdf"));
            Assert.IsFalse(emailValidationAttribute.IsValid("asdf@"));
            Assert.IsFalse(emailValidationAttribute.IsValid("asdf@mail"));
            Assert.IsFalse(emailValidationAttribute.IsValid("asdf@mail."));
            Assert.IsFalse(emailValidationAttribute.IsValid("asdf@mail.c"));
        }
    }
}
