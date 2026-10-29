using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void GetCurrentTsar_ShouldReturnTsarEquivalentToExpected_WhenCalled()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Перепишите код на использование Fluent Assertions.
        actualTsar.Should().BeEquivalentTo(expectedTsar,
            options => options.Excluding(subjectInfo => subjectInfo.Path.EndsWith("Id")));

        // Чем лучше:
        // 1. Если в класс Person добавят новое поле, то BeEquivalentTo автоматически начнет его проверять,
        // а в методе AreEqual пришлось бы вручную дописывать условие проверки нового поля, либо тест пропускал бы ошибку,
        // тк проверки на новое поле мы в него не добавили.
        // 2. Повышена читаемость
        // 3. При падении теста FluentAssertions напишет какое именно поле
        // не совпало, например: "Expected Parent.Age to be 28, but found 30".
        // А метод AreEqual просто вернет false, а там уже ищи свищи что не так пошло
        // 4. BeEquivalentTo корректно обрабатывает циклические ссылки,
        // а рекурсивный AreEqual в таком случае упадет с StackOverflowException
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void AreEqual_ShouldReturnTrue_WhenActualTsarMatchesExpected()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода?
        // - При изменении полей в Person метод придётся переписать с учетом этих нововведений
        // - Нет защиты от циклических ссылок: если Parent будет ссылаться на потомка,
        // рекурсия никогда не завершится и упадет с StackOverflowException
        // - Проверка Id не исключена, хотя Id генерируется новый у каждого Person
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    [Test]
    public void BeEquivalentTo_ShouldIgnoreIdsAtAnyNestingDepth_WhenComparingPersons()
    {
        // Две независимые цепочки из 3 Person: Id не совпадают на всех уровнях
        var actual = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60,
                new Person("Ivan III of Russia", 65, 170, 70, null)));
        var expected = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60,
                new Person("Ivan III of Russia", 65, 170, 70, null)));

        actual.Should().BeEquivalentTo(expected,
            options => options.Excluding(si => si.Path.EndsWith("Id")));
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
