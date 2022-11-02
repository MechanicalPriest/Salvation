using NUnit.Framework;
using Salvation.Core.Modelling.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.CoreTests.Common
{
    internal class AveragedSpellCastResultTest
    {
        AveragedSpellCastResult _result;

        [SetUp]
        public void Init()
        {
            _result = new AveragedSpellCastResult()
            {
                NumberOfHealingTargets = 234,
                NumberOfDamageTargets = 612,
                CastsPerMinute = 22,
                MaximumCastsPerMinute = 623,
                SpellId = 12112,
                SpellName = "Fancy Spell Name",
                RawHealing = 11113,
                Healing = 10032,
                Overhealing = 1855,
                Damage = 864,
                CastTime = 4.32,
                Cooldown = 234,
                Duration = 9,
                Gcd = 3.52,
                ManaCost = 874766,
                Mp5 = 18
            };
        }

        [Test]
        public void RawHPCT()
        {
            // Arrange

            // Act
            var value = _result.RawHPCT;

            // Assert
            Assert.That(value, Is.EqualTo(2572.4537037037035d));
        }

        [Test]
        public void RawHPM()
        {
            // Arrange

            // Act
            var value = _result.RawHPM;

            // Assert
            Assert.That(value, Is.EqualTo(0.012703968832807859d));
        }

        [Test]
        public void RawHPS()
        {
            // Arrange

            // Act
            var value = _result.RawHPS;

            // Assert
            Assert.That(value, Is.EqualTo(4074.7666666666669d));
        }

        [Test]
        public void HPCT()
        {
            // Arrange

            // Act
            var value = _result.HPCT;

            // Assert
            Assert.That(value, Is.EqualTo(2322.2222222222222d));
        }

        [Test]
        public void HPM()
        {
            // Arrange

            // Act
            var value = _result.HPM;

            // Assert
            Assert.That(value, Is.EqualTo(0.011468209784102262d));
        }

        [Test]
        public void HPS()
        {
            // Arrange

            // Act
            var value = _result.HPS;

            // Assert
            Assert.That(value, Is.EqualTo(3678.4000000000001d));
        }

        [Test]
        public void OPS()
        {
            // Arrange

            // Act
            var value = _result.OPS;

            // Assert
            Assert.That(value, Is.EqualTo(680.16666666666663d));
        }

        [Test]
        public void OverhealingPercent()
        {
            // Arrange

            // Act
            var value = _result.OverhealingPercent;

            // Assert
            Assert.That(value, Is.EqualTo(0.1669216233240349d));
        }

        [Test]
        public void MPS()
        {
            // Arrange

            // Act
            var value = _result.MPS;

            // Assert
            Assert.That(value, Is.EqualTo(320751.1333333333d));
        }

        [Test]
        public void DPS()
        {
            // Arrange

            // Act
            var value = _result.DPS;

            // Assert
            Assert.That(value, Is.EqualTo(316.80000000000001d));
        }

        [Test]
        public void DPM()
        {
            // Arrange

            // Act
            var value = _result.DPM;

            // Assert
            Assert.That(value, Is.EqualTo(0.0009876927086786638d));
        }

        [Test]
        public void RawHPCT_CastTime_Zero()
        {
            // Arrange
            _result.CastTime = 0;

            // Act
            var value = _result.RawHPCT;

            // Assert
            Assert.That(value, Is.EqualTo(3157.1022727272725d));
        }

        [Test]
        public void RawHPCT_CastTime_Gcd_Zero()
        {
            // Arrange
            _result.CastTime = 0;
            _result.Gcd = 0;

            // Act
            var value = _result.RawHPCT;

            // Assert
            Assert.That(value, Is.EqualTo(0.0d));
        }

        [Test]
        public void RawHPM_ManaCost_Zero()
        {
            // Arrange
            _result.ManaCost = 0;

            // Act
            var value = _result.RawHPM;

            // Assert
            Assert.That(value, Is.EqualTo(0.0d));
        }

        [Test]
        public void HPCT_CastTime_Zero()
        {
            // Arrange
            _result.CastTime = 0;

            // Act
            var value = _result.HPCT;

            // Assert
            Assert.That(value, Is.EqualTo(2850.0d));
        }

        [Test]
        public void HPCT_CastTime_Gcd_Zero()
        {
            // Arrange
            _result.CastTime = 0;
            _result.Gcd = 0;

            // Act
            var value = _result.HPCT;

            // Assert
            Assert.That(value, Is.EqualTo(0.0d));
        }

        [Test]
        public void HPM_ManaCost_Zero()
        {
            // Arrange
            _result.ManaCost = 0;

            // Act
            var value = _result.HPM;

            // Assert
            Assert.That(value, Is.EqualTo(0.0d));
        }

        [Test]
        public void DPM_ManaCost_Zero()
        {
            // Arrange
            _result.ManaCost = 0;

            // Act
            var value = _result.DPM;

            // Assert
            Assert.That(value, Is.EqualTo(0.0d));
        }

        [Test]
        public void OverhealingPercent_Overhealing_Raw_Zero()
        {
            // Arrange
            _result.Overhealing = 0;
            _result.RawHealing = 0;

            // Act
            var value = _result.OverhealingPercent;

            // Assert
            Assert.That(value, Is.EqualTo(0.0d));
        }

        [Test]
        public void MakeSpellHaveNoCasts_Calculates()
        {
            // Arrange

            // Act
            _result.MakeSpellHaveNoCasts();

            // Assert
            Assert.That(_result.CastsPerMinute, Is.EqualTo(0.0d));
            Assert.That(_result.MaximumCastsPerMinute, Is.EqualTo(0.0d));
        }

        [Test]
        public void MakeCastFree_Calculates()
        {
            // Arrange

            // Act
            _result.MakeCastFree();

            // Assert
            Assert.That(_result.ManaCost, Is.EqualTo(0.0d));
        }

        [Test]
        public void MakeCastInstant_Calculates()
        {
            // Arrange

            // Act
            _result.MakeCastInstant();

            // Assert
            Assert.That(_result.CastTime, Is.EqualTo(0.0d));
        }

        [Test]
        public void MakeCastHaveNoGcd_Calculates()
        {
            // Arrange

            // Act
            _result.MakeCastHaveNoGcd();

            // Assert
            Assert.That(_result.Gcd, Is.EqualTo(0.0d));
        }

        [Test]
        public void ToString_Works_With_Values()
        {
            // Arrange

            // Act
            var value = _result.ToString();

            // Assert
            Assert.That(value, Is.EqualTo("[Fancy Spell Name(id=12112)] RawHPS: 4074.77 HPS: 3678.4 CPM: 22 MaxCPM: 623"));
        }

        [Test]
        public void ToString_Works_With_No_Values()
        {
            // Arrange
            _result = new AveragedSpellCastResult();

            // Act
            var value = _result.ToString();

            // Assert
            Assert.That(value, Is.EqualTo("[(id=0)] RawHPS: 0 HPS: 0 CPM: 0 MaxCPM: 0"));
        }
    }
}
