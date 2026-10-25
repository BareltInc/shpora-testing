using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Перепишите код на использование Fluent Assertions.

        actualTsar.Should().BeEquivalentTo(expectedTsar, 
            options => options.Excluding(person => person.Id)
                                      .Excluding(person => person.Parent.Id));

        // Чем лучше: 
        // 1. Если в класс Person добавят новое поле, то BeEquivalentTo автоматически начнет его проверять,
        // а в методе AreEqual пришлось бы вручную дописывать условие проверки нового поля, либо тест пропускал бы ошибку,
        // тк проверки на новое поле мы в него не добавили.
        // 2. Повышена читаемость
        // 3. При падении теста FluentAssertions напишет какое именно поле 
        // не совпало, например: "Expected Parent.Age to be 28, but found 30".
        // А Метод AreEqual просто вернет false, а там уже ищи свищи что не так пошло
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода?
        // - При изменении полей в Person метод придётся переписать с учетом этих нововведений
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
