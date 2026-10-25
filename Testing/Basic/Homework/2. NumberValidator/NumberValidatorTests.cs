using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    // Отрицательный precision
    [TestCase(-1, 2, true)]
    [TestCase(-1, 2, false)]
    // Нулевой precision
    [TestCase(0, 0, true)]
    // Отрицательный scale
    [TestCase(5, -1, true)]
    // scale больше precision
    [TestCase(3, 5, true)]
    public void Constructor_InvalidArguments_ThrowsArgumentException(int precision, int scale, bool onlyPositive)
    {
        Action act = () => new NumberValidator(precision, scale, onlyPositive);
        act.Should().Throw<ArgumentException>();
    }

    // Минимальные параметры
    [TestCase(1, 0, true)]
    // Корректные параметры
    [TestCase(10, 3, true)]
    [TestCase(10, 3, false)]
    public void Constructor_ValidArguments_DoesNotThrowArgumentException(int precision, int scale, bool onlyPositive)
    {
        Action act = () => new NumberValidator(precision, scale, onlyPositive);
        act.Should().NotThrow<ArgumentException>();
    }

    // Корректные числа
    [TestCase(17, 2, true, "0.0", true)]   // Ноль с дробной частью
    [TestCase(17, 2, true, "0", true)]     // Ноль без дробной части
    [TestCase(4, 2, true, "+1.23", true)]  // Число со знаком плюс
    [TestCase(5, 2, false, "-1.23", true)] // Число со знаком минус
    [TestCase(5, 2, false, "123.45", true)]// Число, использующее весь лимит precision и scale
    // Превышение precision
    [TestCase(3, 2, true, "00.00", false)]
    [TestCase(3, 2, true, "+1.23", false)]
    [TestCase(3, 2, true, "+0.00", false)]
    [TestCase(3, 2, true, "-0.00", false)]
    // Превышение scale 
    [TestCase(17, 2, true, "0.000", false)]
    // Отрицательное число при onlyPositive=true
    // * Было ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));, поменял пресижн с 3 на 4 чтоб была проверка на onlyPositive, а не а на превышение precision
    [TestCase(4, 2, true, "-1.23", false)] 
    // Нечисловые символы
    [TestCase(3, 2, true, "a.sd", false)]
    // Пустая строка
    [TestCase(17, 2, true, "", false)]
    // Null
    [TestCase(17, 2, true, null, false)]
    // Превышение общей длины
    [TestCase(4, 2, true, "123.45", false)]
    public void IsValidNumber_ReturnsExpectedResult(int precision, int scale, bool onlyPositive, string value, bool expectedResult)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().Be(expectedResult,
            $"Тест упал на значении: '{value}' при параметрах precision={precision}, scale={scale}, onlyPositive={onlyPositive}");
    }
}