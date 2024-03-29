﻿using System;
using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Extensions;

namespace Vts.Test.MonteCarlo.Extensions
{
    [TestFixture]
    public class DatabaseExtensionMethodsTests
    {
        /// <summary>
        /// Validate method IspMCDatabase
        /// </summary>
        [Test]
        public void Validate_IspMCDatabase_returns_correct_values()
        {
            // validate those that are true
            var databaseType = DatabaseType.pMCDiffuseReflectance;
            Assert.IsTrue(databaseType.IspMCDatabase());
            databaseType = DatabaseType.pMCDiffuseTransmittance;
            Assert.IsTrue(databaseType.IspMCDatabase());
            // validate those that are false
            databaseType = DatabaseType.DiffuseReflectance;
            Assert.IsFalse(databaseType.IspMCDatabase());
            databaseType = DatabaseType.DiffuseTransmittance;
            Assert.IsFalse(databaseType.IspMCDatabase());
            databaseType = DatabaseType.SpecularReflectance;
            Assert.IsFalse(databaseType.IspMCDatabase());
            // check if enum set to something out of range
            databaseType = (DatabaseType)Enum.GetNames(typeof(DatabaseType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => databaseType.IspMCDatabase());
        }

        /// <summary>
        /// Validate method GetCorrespondingVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_GetCorrespondingVirtualBoundary_returns_correct_values()
        {
            var databaseType = DatabaseType.DiffuseReflectance;
            var virtualBoundary = databaseType.GetCorrespondingVirtualBoundaryType();
            Assert.AreEqual(VirtualBoundaryType.DiffuseReflectance, virtualBoundary);
            databaseType = DatabaseType.DiffuseTransmittance;
            virtualBoundary = databaseType.GetCorrespondingVirtualBoundaryType();
            Assert.AreEqual(VirtualBoundaryType.DiffuseTransmittance, virtualBoundary);
            databaseType = DatabaseType.SpecularReflectance;
            virtualBoundary = databaseType.GetCorrespondingVirtualBoundaryType();
            Assert.AreEqual(VirtualBoundaryType.SpecularReflectance, virtualBoundary);
            databaseType = DatabaseType.pMCDiffuseReflectance;
            virtualBoundary = databaseType.GetCorrespondingVirtualBoundaryType();
            Assert.AreEqual(VirtualBoundaryType.pMCDiffuseReflectance, virtualBoundary);
            databaseType = DatabaseType.pMCDiffuseTransmittance;
            virtualBoundary = databaseType.GetCorrespondingVirtualBoundaryType();
            Assert.AreEqual(VirtualBoundaryType.pMCDiffuseTransmittance, virtualBoundary);
            // check if enum set to something out of range
            databaseType = (DatabaseType)Enum.GetNames(typeof(DatabaseType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => databaseType.GetCorrespondingVirtualBoundaryType());

        }
    }
}

