using FluentAssertions;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(-1, 2, TestName = "Отрицательный precision")]
    [TestCase(-10, 2, TestName = "Непограничное отрицательное число")]
    [TestCase(-1, -1, TestName = "Оба аргумента отрицательные")]
    [TestCase(0, 0, TestName = "Нулевой precision")]
    [TestCase(0, 2, TestName = "Нулевой precision и положительный scale")]
    [TestCase(5, -1, TestName = "Отрицательный scale")]
    [TestCase(5, -10, TestName = "Непограничный отрицательный scale")]
    [TestCase(3, 5, TestName = "scale больше precision")]
    [TestCase(3, 3, TestName = "scale равно precision")]
    [TestCase(int.MinValue, 2, TestName = "Очень маленькое число (int.MinValue)")]
    [TestCase(5, int.MinValue, TestName = "Очень маленький scale (int.MinValue)")]
    [TestCase(int.MaxValue, int.MaxValue, TestName = "Большие числа (int.MaxValue)")]
    public void Constructor_ShouldThrowArgumentException_WhenInvalidArgumentsPassed(int precision, int scale)
    {
        Action act = () => new NumberValidator(precision, scale);
        act.Should().Throw<ArgumentException>();
    }

    [TestCase(17, 2, true, "0.0", TestName = "Ноль с дробной частью")]
    [TestCase(17, 2, true, "0", TestName = "Ноль без дробной части")]
    [TestCase(4, 2, true, "+1.23", TestName = "Дробное число со знаком плюс")]
    [TestCase(4, 0, true, "+1", TestName = "Целое число со знаком плюс")]
    [TestCase(5, 2, false, "-1.23", TestName = "Число со знаком минус")]
    [TestCase(3, 0, false, "-12", TestName = "Отрицательное целое, использующее весь precision")]
    [TestCase(5, 2, false, "123.45", TestName = "Число, использующее весь precision и scale")]
    [TestCase(3, 0, true, "1", TestName = "Целое число без знака")]
    [TestCase(4, 0, false, "-1", TestName = "Целое число со знаком минус")]
    [TestCase(4, 2, true, "1.23", TestName = "Дробное число без знака")]
    [TestCase(5, 2, true, "01.23", TestName = "Число начинается с нуля")]
    [TestCase(5, 2, false, "12,34", TestName = "Запятая как разделитель дробной части")]
    [TestCase(17, 2, true, "١٢.٣٤", TestName = "Неарабские цифры")] // Арабы кста пишут этими цифрами
    public void IsValidNumber_ShouldReturnTrue_WhenNumberIsValid(int precision, int scale, bool onlyPositive, string value)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase(3, 2, true, "00.00", TestName = "Превышение precision (ведущие нули)")]
    [TestCase(3, 2, true, "+1.23", TestName = "Превышение precision (знак плюс)")]
    [TestCase(3, 2, true, "+0.00", TestName = "Превышение precision (положительный ноль со знаком)")]
    [TestCase(3, 2, true, "-0.00", TestName = "Превышение precision (отрицательный ноль со знаком)")]
    [TestCase(4, 2, true, "123.45", TestName = "Превышение общей длины")]
    [TestCase(17, 2, true, "0.000", TestName = "Превышение scale")]
    [TestCase(4, 2, true, "-1.23", TestName = "Отрицательное число при onlyPositive=true")]
    [TestCase(3, 2, true, "a.sd", TestName = "Нечисловые символы")]
    [TestCase(5, 2, true, "1a.23", TestName = "Число, с буквами")]
    [TestCase(17, 2, true, "", TestName = "Пустая строка")]
    [TestCase(17, 2, true, null, TestName = "Null")]
    [TestCase(17, 2, true, "null", TestName = "Null как строка")]
    [TestCase(5, 2, true, "1.", TestName = "Число, оканчивающееся на точку")]
    [TestCase(5, 2, true, ".23", TestName = "Число, начинающееся с точки")]
    [TestCase(5, 2, true, "+", TestName = "Только знак")]
    [TestCase(17, 2, true, " 1.23", TestName = "Число с пробелом")]
    [TestCase(17, 2, true, "1.2.3", TestName = "Несколько разделителей")]
    public void IsValidNumber_ShouldReturnFalse_WhenNumberIsInvalid(int precision, int scale, bool onlyPositive, string value)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        validator.IsValidNumber(value).Should().BeFalse();
    }
}
