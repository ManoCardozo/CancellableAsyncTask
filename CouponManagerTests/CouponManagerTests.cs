using Moq;
using System;
using NUnit.Framework;
using CouponManagerLibrary;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CouponManagerTests
{
    [TestFixture]
    public class CouponManagerTests
    {
        private readonly Mock<ILogger> mockLogger = new Mock<ILogger>();
        private readonly Mock<ICouponProvider> mockCouponProvider = new Mock<ICouponProvider>();

        private Coupon coupon;
        private CouponManager couponManager;
        private List<Func<Coupon, Guid, bool>> sucessEvaluators;
        private List<Func<Coupon, Guid, bool>> failEvaluators;
        private List<Func<Coupon, Guid, bool>> emptyEvaluator;

        public CouponManagerTests()
        {

        }

        [SetUp]
        public void Setup()
        {
            couponManager = new CouponManager(mockLogger.Object, mockCouponProvider.Object);

            coupon = new Coupon();

            emptyEvaluator = new List<Func<Coupon, Guid, bool>>();

            sucessEvaluators = new List<Func<Coupon, Guid, bool>>()
            {
                (c, g) => { return true; },
                (c, g) => { return false; }
            };

            failEvaluators = new List<Func<Coupon, Guid, bool>>()
            {
                (c, g) => { return false; },
                (c, g) => { return false; }
            };
        }

        [Test]
        public void CouponManager_Must_Receive_ILogger()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => new CouponManager(null, mockCouponProvider.Object));
        }

        [Test]
        public void CouponManager_Must_Receive_ICouponProvider()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => new CouponManager(mockLogger.Object, null));
        }

        [Test]
        public void CanRedeemCoupon_Should_Not_Accept_Null_Valuators()
        {
            //Arrange
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => couponManager.CanRedeemCoupon(couponId, userId, null));
        }

        [Test]
        public void CanRedeemCoupon_Should_Error_If_Invalid_Coupon()
        {
            //Arrange
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            mockCouponProvider
                .Setup(m => m.Retrieve(couponId))
                .ReturnsAsync(() => null);

            // Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => couponManager.CanRedeemCoupon(couponId, userId, sucessEvaluators));
        }

        [Test]
        public async Task CanRedeemCoupon_Should_Allow_Redemption_If_No_Valuators()
        {
            //Arrange
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            mockCouponProvider
                .Setup(m => m.Retrieve(couponId))
                .ReturnsAsync(coupon);

            // Act
            var canRedeem = await couponManager.CanRedeemCoupon(couponId, userId, emptyEvaluator);

            // Assert
            Assert.IsTrue(canRedeem);
        }

        [Test]
        public async Task CanRedeemCoupon_Should_Allow_Redemption()
        {
            //Arrange
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            mockCouponProvider
                .Setup(m => m.Retrieve(couponId))
                .ReturnsAsync(coupon);

            // Act
            var canRedeem = await couponManager.CanRedeemCoupon(couponId, userId, sucessEvaluators);

            // Assert
            Assert.IsTrue(canRedeem);
        }

        [Test]
        public async Task CanRedeemCoupon_Should_Disallow_Redemption()
        {
            //Arrange
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            mockCouponProvider
                .Setup(m => m.Retrieve(couponId))
                .ReturnsAsync(coupon);

            // Act
            var canRedeem = await couponManager.CanRedeemCoupon(couponId, userId, failEvaluators);

            // Assert
            Assert.IsFalse(canRedeem);
        }
    }
}
