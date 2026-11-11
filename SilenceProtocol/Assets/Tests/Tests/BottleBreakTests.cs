using NUnit.Framework;
using UnityEngine;

public class BottleBreakTests
{
    private GameObject testObject;
    private BottleBreak bottle;

    [SetUp]
    public void SetUp()
    {
        testObject = new GameObject("TestBottle");
        bottle = testObject.AddComponent<BottleBreak>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testObject);
    }

    [Test]
    public void NewBottle_StartsUnarmed()
    {
        Assert.That(bottle.IsArmed, Is.False);
    }

    [Test]
    public void Arm_EnablesBreaking()
    {
        bottle.Arm();

        Assert.That(bottle.IsArmed, Is.True);
    }

    [Test]
    public void Disarm_AfterArming_DisablesBreaking()
    {
        bottle.Arm();
        bottle.Disarm();

        Assert.That(bottle.IsArmed, Is.False);
    }
}