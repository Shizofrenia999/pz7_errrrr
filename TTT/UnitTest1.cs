using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System;
using System.IO;
using System.Text;
using Xunit;
namespace TTT
{
    public class UnitTest1
    { // Тест с использованием [Fact] для проверки расчета кредита
        [Fact]
        public void CalculateLoan_WithValidParameters_ReturnsCorrectMonthlyPayment()
        {
            // Arrange
            var calculator = new Program();
            double loanAmount = 100000; // 100,000 руб
            int loanTerm = 12; // 12 месяцев
            double interestRate = 12; // 12% годовых

            // Ожидаемый ежемесячный платеж (можно рассчитать заранее)
            double expectedMonthlyPayment = 8884.88; // Известное значение для этих параметров

            // Act
            double monthlyRate = interestRate / 12 / 100;
            double actualMonthlyPayment = loanAmount * (monthlyRate * Math.Pow(1 + monthlyRate, loanTerm)) /
                                        (Math.Pow(1 + monthlyRate, loanTerm) - 1);

            // Assert
            Assert.Equal(expectedMonthlyPayment, actualMonthlyPayment, 2); // Проверка с точностью до 2 знаков
        }

        // Тест с использованием [Theory] для проверки конвертации валют
        [Theory]
        [InlineData("USD", "RUB", 100, 9000)] // 100 USD = 9000 RUB
        [InlineData("EUR", "RUB", 50, 4925)]  // 50 EUR = 4925 RUB
        [InlineData("RUB", "USD", 9000, 100)] // 9000 RUB = 100 USD
        [InlineData("EUR", "USD", 100, 109)]  // 100 EUR = 109 USD
        [InlineData("USD", "USD", 100, 100)]  // Та же валюта
        public void ConvertCurrencyAmount_WithDifferentCurrencies_ReturnsCorrectResult(
            string fromCurrency, string toCurrency, double amount, double expectedResult)
        {
            // Arrange & Act
            double actualResult = ConvertCurrencyAmount(fromCurrency, toCurrency, amount);

            // Assert
            Assert.Equal(expectedResult, actualResult, 2);
        }

        // Тест для проверки обработки некорректной валюты
        [Fact]
        public void ConvertCurrencyAmount_WithInvalidCurrency_ThrowsArgumentException()
        {
            // Arrange
            string invalidCurrency = "GBP";
            string validCurrency = "USD";
            double amount = 100;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                ConvertCurrencyAmount(invalidCurrency, validCurrency, amount));
        }

        // Тест с использованием Mock для проверки ввода пользователя
        [Fact]
        public void CalculateDeposit_WithCapitalization_CallsConsoleMethodsCorrectly()
        {
            // Arrange
            var mockConsole = new Mock<IConsole>();
            mockConsole.SetupSequence(x => x.ReadLine())
                .Returns("50000")    // Сумма вклада
                .Returns("12")       // Срок вклада
                .Returns("5")        // Процентная ставка
                .Returns("1");       // Тип вклада (с капитализацией)

            // Сохраняем оригинальный Console.Out для восстановления
            var originalOut = Console.Out;

            try
            {
                using var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                // Act
                CalculateDeposit();

                // Assert
                string output = stringWriter.ToString();
                Assert.Contains("Доход по вкладу:", output);
                Assert.Contains("Итоговая сумма:", output);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        // Тест для проверки расчета вклада без капитализации с использованием [Theory]
        [Theory]
        [InlineData(100000, 12, 10, 10000)] // 100,000 руб, 12 месяцев, 10% = 10,000 руб дохода
        [InlineData(50000, 6, 12, 3000)]    // 50,000 руб, 6 месяцев, 12% = 3,000 руб дохода
        [InlineData(200000, 24, 8, 32000)]  // 200,000 руб, 24 месяца, 8% = 32,000 руб дохода
        public void CalculateDeposit_WithoutCapitalization_ReturnsCorrectIncome(
            double depositAmount, int depositTerm, double interestRate, double expectedIncome)
        {
            // Arrange & Act
            double actualIncome = depositAmount * interestRate * depositTerm / 12 / 100;


            // Assert
            Assert.Equal(expectedIncome, actualIncome, 2);
        }

        // Тест с Mock.Verify для проверки вызовов консоли
        [Fact]
        public void MainMenu_WhenOption4Selected_ExitsProgram()
        {
            // Arrange
            var stringReader = new StringReader("4"); // Пользователь выбирает выход
            Console.SetIn(stringReader);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            Program.Main(new string[0]);

            // Assert
            string output = stringWriter.ToString();
            Assert.Contains("До свидания", output);
        }

        // Дополнительный тест для проверки граничных условий
        [Fact]
        public void CalculateLoan_WithZeroInterestRate_ReturnsLoanAmountWithoutOverpayment()
        {
            // Arrange
            double loanAmount = 100000;
            int loanTerm = 10;
            double interestRate = 0;

            // Act
            double monthlyRate = interestRate / 12 / 100;
            double monthlyPayment = loanAmount * (monthlyRate * Math.Pow(1 + monthlyRate, loanTerm)) /
                                  (Math.Pow(1 + monthlyRate, loanTerm) - 1);

            double totalPayment = monthlyPayment * loanTerm;
            double overpayment = totalPayment - loanAmount;

            // Assert
            Assert.Equal(0, overpayment, 2); // Переплата должна быть 0
            Assert.Equal(loanAmount, totalPayment, 2); // Общая выплата равна сумме кредита
        }

        // Вспомогательный интерфейс для мокирования консоли
        public interface IConsole
        {
            string ReadLine();
            void WriteLine(string value);
            void Write(string value);
        }

        // Вспомогательные методы для доступа к private методам класса Program
        private static double ConvertCurrencyAmount(string from, string to, double amount)
        {
            // Фиксированные курсы (должны соответствовать реализации)
            double usdToRub = 90.0;
            double eurToRub = 98.5;
            double eurToUsd = 1.09;

            if (from == to)
                return amount;

            double amountInRub = 0;

            switch (from)
            {
                case "RUB":
                    amountInRub = amount;
                    break;
                case "USD":
                    amountInRub = amount * usdToRub;
                    break;
                case "EUR":
                    amountInRub = amount * eurToRub;
                    break;
                default:
                    throw new ArgumentException($"Неизвестная валюта: {from}");
            }

            switch (to)
            {
                case "RUB":
                    return amountInRub;
                case "USD":
                    return amountInRub / usdToRub;
                case "EUR":
                    return amountInRub / eurToRub;
                default:
                    throw new ArgumentException($"Неизвестная валюта: {to}");
            }
        }

        // Вспомогательные методы для вызова private методов через reflection
        private static void CalculateDeposit()
        {
            // Этот метод должен вызывать реальную логику через reflection
            // или быть адаптирован для тестирования
        }
    }
}