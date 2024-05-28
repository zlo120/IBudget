using IBudget.Core.Model;

namespace IBudget.Tests
{
    public class UserExpenseDictionaryTests
    {
        [Fact]
        public void ExpenseDictionariesAreEqual()
        {
            // Arrange
            var expenseDictionary1 = new ExpenseDictionary()
            {
                title = "ED1",
                tags = ["tag1", "tag2"]
            };
            var expenseDictionary2 = new ExpenseDictionary()
            {
                title = "ED1",
                tags = ["tag1", "tag2"]
            };

            var expenseDictionary3 = new ExpenseDictionary()
            {
                title = "ED2",
                tags = ["tag1", "tag2"]
            };
            var expenseDictionary4 = new ExpenseDictionary()
            {
                title = "ED2",
                tags = ["tag3", "tag4"]
            };

            // Act & Assert
            Assert.True(expenseDictionary1 == expenseDictionary2);            
            Assert.True(expenseDictionary3 == expenseDictionary4);
        }
        [Fact]
        public void ExpenseDictionariesAreNotEqual()
        {
            var expenseDictionary1 = new ExpenseDictionary()
            {
                title = "ED1",
                tags = ["tag1", "tag2"]
            };
            var expenseDictionary2 = new ExpenseDictionary()
            {
                title = "ED2",
                tags = ["tag3", "tag2"]
            };

            var expenseDictionary3 = new ExpenseDictionary()
            {
                title = "ED3",
                tags = ["tag1", "tag2"]
            };
            var expenseDictionary4 = new ExpenseDictionary()
            {
                title = "ED4",
                tags = ["tag1", "tag2"]
            };

            Assert.False(expenseDictionary1 == expenseDictionary2);
            Assert.False(expenseDictionary2 == expenseDictionary3);
            Assert.True(expenseDictionary1 != expenseDictionary2);
            Assert.True(expenseDictionary3 != expenseDictionary4);
        }
        [Fact]
        public void UserExpenseDictionariesAreEqual()
        {
            var userExpenseDictionary1 = new UserDictionary()
            {
                userId = 1,
                ExpenseDictionaries = new List<ExpenseDictionary>()
                {
                    new ExpenseDictionary()
                    {
                        title = "ED1",
                        tags = ["tag1", "tag2"]
                    },
                    new ExpenseDictionary()
                    {
                        title = "ED2",
                        tags = ["tag3", "tag4"]
                    }
                }
            };
            var userExpenseDictionary2 = new UserDictionary()
            {
                userId = 1,
                ExpenseDictionaries = new List<ExpenseDictionary>()
                {
                    new ExpenseDictionary()
                    {
                        title = "ED1",
                        tags = ["tag1", "tag2"]
                    },
                    new ExpenseDictionary()
                    {
                        title = "ED2",
                        tags = ["tag3", "tag4"]
                    }
                }
            };
            
            Assert.True(userExpenseDictionary1 == userExpenseDictionary2);
        }
        [Fact]
        public void UserExpenseDictionariesAreNotEqual()
        {

            var userExpenseDictionary1 = new UserDictionary()
            {
                userId = 1,
                ExpenseDictionaries = new List<ExpenseDictionary>()
                {
                    new ExpenseDictionary()
                    {
                        title = "ED1",
                        tags = ["tag1", "tag2"]
                    },
                    new ExpenseDictionary()
                    {
                        title = "ED2",
                        tags = ["tag3", "tag4"]
                    }
                }
            };
            var userExpenseDictionary2 = new UserDictionary()
            {
                userId = 2,
                ExpenseDictionaries = new List<ExpenseDictionary>()
                {
                    new ExpenseDictionary()
                    {
                        title = "ED1",
                        tags = ["tag1", "tag2"]
                    },
                    new ExpenseDictionary()
                    {
                        title = "ED2",
                        tags = ["tag3", "tag4"]
                    }
                }
            };
        
            Assert.True(userExpenseDictionary1 != userExpenseDictionary2);
            Assert.False(userExpenseDictionary1 == userExpenseDictionary2);
        }
    }
}