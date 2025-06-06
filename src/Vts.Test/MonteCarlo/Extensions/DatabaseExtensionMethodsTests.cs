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
            Assert.That(databaseType.IspMCDatabase(), Is.True);
            databaseType = DatabaseType.pMCDiffuseTransmittance;
            Assert.That(databaseType.IspMCDatabase(), Is.True);
            // validate those that are false
            databaseType = DatabaseType.DiffuseReflectance;
            Assert.That(databaseType.IspMCDatabase(), Is.False);
            databaseType = DatabaseType.DiffuseTransmittance;
            Assert.That(databaseType.IspMCDatabase(), Is.False);
            databaseType = DatabaseType.SpecularReflectance;
            Assert.That(databaseType.IspMCDatabase(), Is.False);
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
            Assert.That(virtualBoundary, Is.EqualTo(VirtualBoundaryType.DiffuseReflectance));
            databaseType = DatabaseType.DiffuseTransmittance;
            virtualBoundary = databaseType.GetCorrespondingVirtualBoundaryType();
            Assert.That(virtualBoundary, Is.EqualTo(VirtualBoundaryType.DiffuseTransmittance));
            databaseType = DatabaseType.SpecularReflectance;
            virtualBoundary = databaseType.GetCorrespondingVirtualBoundaryType();
            Assert.That(virtualBoundary, Is.EqualTo(VirtualBoundaryType.SpecularReflectance));
            databaseType = DatabaseType.pMCDiffuseReflectance;
            virtualBoundary = databaseType.GetCorrespondingVirtualBoundaryType();
            Assert.That(virtualBoundary, Is.EqualTo(VirtualBoundaryType.pMCDiffuseReflectance));
            databaseType = DatabaseType.pMCDiffuseTransmittance;
            virtualBoundary = databaseType.GetCorrespondingVirtualBoundaryType();
            Assert.That(virtualBoundary, Is.EqualTo(VirtualBoundaryType.pMCDiffuseTransmittance));
            // check if enum set to something out of range
            databaseType = (DatabaseType)Enum.GetNames(typeof(DatabaseType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => databaseType.GetCorrespondingVirtualBoundaryType());

        }
    }
}

