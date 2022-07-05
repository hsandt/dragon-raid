// Copied from https://bitbucket.org/hsandt/unity-commons-ai/src/4fe1d26198026f7f4f1fb612af3a1cc6dec0a702/Movement/PhysicsPrediction/PhysicsPrediction.cs
// along with PhysicsPredictionTests.cs

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Comparers;
using System;
using System.Collections;

public static class PhysicsPrediction {

	// absolute comparer may be useful for known values. For instance, distances of less than 100. If more, use relative comparers.
	static FloatComparer absoluteComparer = new FloatComparer(0.01f);  // high tolerance!

	/// Return true if there is a shooting angle to reach a target at end from start, shooting a projectile at muzzleSpeed under gravity,
	/// and set aimDirection to the lower shooting direction if so.
	// IMPROVE: if needed for the game, also out the impact time
	public static bool CalculateFiringSolution(Vector2 start, Vector2 end, float muzzleSpeed, Vector2 gravity, out Vector2 aimDirection) {

		if (muzzleSpeed <= 0f)
			throw new ArgumentException(string.Format("muzzleSpeed is {0}, but cannot be negative or null", muzzleSpeed));

		// Calculate the vector to the target
		Vector2 delta = end - start;

		// Calculate the real-valued a, b, c coefficients of a conventional quadratic equation
		float a = gravity.sqrMagnitude;
		float b = -4 * (Vector2.Dot(gravity, delta) + muzzleSpeed * muzzleSpeed);
		float c = 4 * delta.sqrMagnitude;

		if (a == 0f) {
			Debug.Log("Gravity is null, straight line shot");
			// degenerated case: straight line shot
			aimDirection = delta.normalized;
			// impact time is delta / muzzleSpeed, out it if needed
			return true;
		}

		if (delta.sqrMagnitude == 0f) {
			// target is at same position at shooter
			// there are 2 solutions: shoot in any direction and hit at t = 0, or shoot in the opposite direction of the gravity and
			// wait for the bullet to come back
			// In practice, we rarely check for firing when the target is too close, but if we had to we may indeed shoot upward for a downward gravity,
			// maybe to run away afterward
			aimDirection = - gravity.normalized;
			return true;
		}

		// Check for real solutions
		float discriminant = b * b - 4 * a * c;

		// REFACTOR: merge with 2 roots case
		// Check for null or almost null discriminant (use scaled comparison at 1%)
		if (Mathf.Abs(discriminant) < 0.01f * b * b) {
			Debug.Log("Discriminant is close to zero, 1 root case");
			// This corresponds to the 45° limit case. Since gravity may not be downward and we may shoot left or right,
			// we still compute this with the usual formula.
			float impactTime = Mathf.Sqrt(-b / (2 * a));
			aimDirection = (2 * delta - gravity * impactTime * impactTime) / (2 * muzzleSpeed * impactTime);
			// Assert.AreApproximatelyEqual(1f, aimDirection.sqrMagnitude, "aimDirection is not a unit vector");
			if (Mathf.Abs(aimDirection.sqrMagnitude - 1f) > 0.1f)
				Debug.LogWarningFormat("aimDirection {0} is not a unit vector, magnitude: {1}", aimDirection, aimDirection.magnitude);
			return true;
		}

		else if (discriminant < 0) {
//			Debug.LogFormat("Gravity is too strong orthogonally to expected aim direction, cannot reach target. Discriminant: {0}", discriminant);
			aimDirection = Vector2.zero;
			return false;
		}

		else {
			float impactTime;

			// Find the candidate times (both equal if discriminant == 0)
			// Since we apply the 2nd square root, we must check the sign of the radicand first.
			// Indeed, even if discriminant is positive, the radicand -b +/- sqrt(discriminant) may be negative
			// if the gravity is too strong, in the opposite direction of the expected shooting (roughly delta).
			// For instance, the character is shooting a bird very high or he is shooting forward but there is an attraction point behind him.
			// We also know from Vieta's formula that radicand0 * radicand1 has the same sign as a * c > 0, so checking only one sign should be enough.

			// use the mixed robust formulae depending on sign(b) (https://en.wikipedia.org/wiki/Quadratic_equation)
			// because we use epsilon sign, we don't know which radicand is the bigger one
			// if epsilon > 0, since a > 0, then radicandA <= radicandB (== iff discriminant == 0)
			// else radicandA >= radicandB (== iff discriminant == 0)
			// since a * c >= 0 we also know that radicand0 and radicand1 have the same sign (0 being ambiguous)
			float epsilon = Mathf.Sign(b);
			float radicandA = (-b - epsilon * Mathf.Sqrt(discriminant)) / (2 * a);
			float radicandB = - 2 * c / (b + epsilon * Mathf.Sqrt(discriminant));  // I moved the minus sign to the numerator

			// check sign of epsilon, not b, so that we can decide for the case when b == 0
			float radicand0 = epsilon > 0 ? radicandA : radicandB;
			float radicand1 = epsilon < 0 ? radicandB : radicandA;
			Assert.IsTrue(radicand0 <= radicand1, $"Expected radicand0 <= radicand1, got radicand0: {radicand0} and radicand1: {radicand1}");

			if (radicand0 >= 0) {
				// Since radicand1 > radicand0, both are positive. We pick the smallest time.
				impactTime = Mathf.Sqrt(radicand0);
			}
			else if (radicand1 < 0) {
				Debug.Log("Gravity is too strong opposing expected aim direction, cannot reach target");
				aimDirection = Vector2.zero;
				return false;
			}
			else if (radicand1 == 0) {
				// Limit case where the other radicand is null. Shooting oneself?
				impactTime = Mathf.Sqrt(radicand1);
			} else {
				Debug.LogErrorFormat("Radicand 0 {0} is negative, yet radicand 1 {1} is positive, and both should have the same sign.", radicand0, radicand1);
				// we can still try...
				impactTime = Mathf.Sqrt(radicand1);
			}

			// Return the firing vector
			if (impactTime == 0f) {
				Debug.LogErrorFormat("impactTime is null, cannot compute aimDirection with formula");
				// maybe we are shooting ourshelf, so shoot anywhere
				aimDirection = Vector2.right;
				return true;
			}

			aimDirection = (2 * delta - gravity * impactTime * impactTime) / (2 * muzzleSpeed * impactTime);
//			Debug.LogFormat("Dual root solution, impact time: {0}, aimDirection: {1}", impactTime, LogUtil.VectorToString(aimDirection));
			Assert.AreApproximatelyEqual(1f, aimDirection.sqrMagnitude, "aimDirection is not a unit vector");
			Assert.AreEqual(1f, aimDirection.sqrMagnitude, "aimDirection is not a unit vector", absoluteComparer);
			if (Mathf.Abs(aimDirection.sqrMagnitude - 1f) > 0.1f)
				Debug.LogWarningFormat("aimDirection {0} is not a unit vector, magnitude: {1}", aimDirection, aimDirection.magnitude);
			return true;
		}
	}

}
