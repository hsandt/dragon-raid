// Copied from https://bitbucket.org/hsandt/unity-commons-ai/src/4fe1d26198026f7f4f1fb612af3a1cc6dec0a702/Editor/Tests/PhysicsPredictionTests.cs

using UnityEngine;
using System;
using CommonsHelper;
using NUnit.Framework;

[TestFixture]
public class PhysicsPredictionTests
{

	private static readonly Vector2 origin = Vector2.zero;
	private static readonly Vector2 u = Vector2.right;
	private static readonly Vector2 v = Vector2.up;
	private static readonly Vector2 gravity = 10f * Vector2.down;

	[OneTimeSetUp]
	public void Init ()
	{
	}

	// FIXME: tests are incorrect since physics test in real scenes show that methods are correct,
	// yet tests fail

	[Test]
	public void CalculateFiringSolution_TargetIsStraightUpReachable_UpwardVector()
	{
		// input
		Vector2 start = origin;
		Vector2 end = v;
		
		// Muzzle speed is enough if kinetic energy on launch can be entirely consumed while potential energy
		// is increased to match the height of the target
		// Ek(0) - Ek(apogee) >= Ep(apogee) - Ep(0)
		// 0.5*m*v0^2 >= mgz
		// 0.5 * muzzleSpeed * muzzleSpeed >= gravity * height
		// muzzleSpeed >=  sqrt(2 * gravity * height)
		
		// Hence the shortened "jump" initial speed formula:
		// V_y0 = sqrt(2 * g * height)

		// We want to test the case where we *can* reach the target first
		
		float muzzleSpeed = Mathf.Sqrt(2 * gravity.y * 1f);  // sqrt(20) ~ 4.5

		// expected result
		Vector2 expectedAimDirection = v;

		// computation
		Vector2 aimDirection;
		// this fails due to Assert inside method because radicands are NaN
		bool result = PhysicsPrediction.CalculateFiringSolution(start, end, muzzleSpeed, gravity, out aimDirection);

		// assertion
		Assert.IsTrue(result);
		Assert.That(aimDirection, Is.EqualTo(expectedAimDirection).Using(new VectorDeltaEqualityComparer(0.0001f)));
	}

	[Test]
	public void CalculateFiringSolution_TargetIsStraightUpUnreachable_UpwardVector()
	{
		// input
		Vector2 start = origin;
		Vector2 end = v;
		
		// Same as above, but we want muzzle speed to be so low we cannot reach the target
		// We want to test the case where we *can* reach the target first
		
		float muzzleSpeed = Mathf.Sqrt(2 * gravity.y * 1f) - 0.1f;  // ~ 4.4, just below min speed required to reach

		// expected result
		Vector2 expectedAimDirection = v;

		// computation
		Vector2 aimDirection;
		// this fails due to Assert inside method because radicands are NaN
		bool result = PhysicsPrediction.CalculateFiringSolution(start, end, muzzleSpeed, gravity, out aimDirection);

		// assertion
		Assert.IsFalse(result);
	}

	// This test is wrong
	[Test]
	public void CalculateFiringSolution_StartAtOriginTargetAtLimitRangeSameY_45DegVector()
	{
		// input
		Vector2 start = Vector2.zero;
		Vector2 end = new Vector2(0.1f / Mathf.Sqrt(2), 0f);
		float muzzleSpeed = 1f;

		// expected result
		Vector2 expectedAimDirection = 1 / Mathf.Sqrt(2) * Vector2.one;

		// computation
		Vector2 aimDirection;
		bool result = PhysicsPrediction.CalculateFiringSolution(start, end, muzzleSpeed, gravity, out aimDirection);

		// assertion
		Assert.IsTrue(result);
		Assert.That(aimDirection, Is.EqualTo(expectedAimDirection).Using(new VectorDeltaEqualityComparer(0.0001f)));
	}

	// This test is wrong
	[Test]
	public void CalculateFiringSolution_TargetAtLimitRangeSameY_45DegVector()
	{
		// Vector2 expectedAimDirection = 1 / Mathf.Sqrt(2) * Vector2.one;
		Vector2 expectedAimDirection = Vector2.one.normalized;
		float muzzleSpeed = 2f;
		// we can find the end we need for a solution of 45 degrees with backward reasoning
		// (find out landing spot on flat terrain)
		float t = - 2 * expectedAimDirection.y * muzzleSpeed / gravity.y;
		Vector2 start = u;
		Debug.LogFormat("start: {0}", start);
		// landing sport on flat ground (ignore vertical gravity, just compute deltaX)
		Vector2 end = start + expectedAimDirection.x * muzzleSpeed * t * Vector2.right - Vector2.right * 0.2f;
		Debug.LogFormat("end: {0}", end);
		// end = new Vector2(1f + Mathf.Sqrt(2) * 2f * t + 0.5 * -10f * t * t, 0);
		// 1 + sqrt(2) * 2 * sqrt(2) * 2 / 10 - 5 * 2 * 4 / 100
		// = 1 + 2 * 4 / 10 - 10 * 4 / 100
		// = 1 + 0.8 - 0.4
		// = 1.4
		// Vector2 end = 1.4f;
		Vector2 aimDirection;
		bool result = PhysicsPrediction.CalculateFiringSolution(start, end, muzzleSpeed, gravity, out aimDirection);
		Assert.IsTrue(result);
		Assert.That(aimDirection, Is.EqualTo(expectedAimDirection).Using(new VectorDeltaEqualityComparer(0.0001f)));
	}
}
