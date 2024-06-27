using System;
using System.Collections.Generic;
using System.Windows;

namespace RecipeApp
{
    public class Recipe
    {
        public string Name { get; set; }
        public int IngredientCount { get; set; }
        public int StepCount { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        private List<string> Steps { get; set; }
        private List<double> OriginalQuantities { get; set; }
        private List<string> OriginalUnits { get; set; }

        public delegate void CaloriesExceededEventHandler(string recipeName, double totalCalories);
        public event CaloriesExceededEventHandler CaloriesExceeded;

        public Recipe(string name, int ingredientCount, int stepCount)
        {
            Name = name;
            IngredientCount = ingredientCount;
            StepCount = stepCount;
            Ingredients = new List<Ingredient>(ingredientCount);
            OriginalQuantities = new List<double>(ingredientCount);
            OriginalUnits = new List<string>(ingredientCount);
            Steps = new List<string>(stepCount);
        }

        public void GetIngredients(int ingredientCount)
        {
            for (int i = 0; i < ingredientCount; i++)
            {
                string name = Prompt($"Enter ingredient {i + 1} name:");
                double quantity = GetPositiveDouble("Enter quantity:");
                string unit = Prompt("Enter unit:");
                double calories = GetPositiveDouble("Enter calories:");
                string foodGroup = Prompt("Enter food group:");

                Ingredients.Add(new Ingredient { Name = name, Quantity = quantity, Unit = unit, Calories = calories, FoodGroup = foodGroup });
            }
        }

        private string Prompt(string message)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(message, "Input");
        }

        public double GetPositiveDouble(string prompt)
        {
            double value;
            while (true)
            {
                string input = Prompt(prompt);
                if (double.TryParse(input, out value) && value > 0)
                {
                    break;
                }
                MessageBox.Show("Invalid input. Please enter a positive number.");
            }
            return value;
        }

        public void GetSteps(int stepCount)
        {
            for (int i = 0; i < stepCount; i++)
            {
                string step = Prompt($"Enter the description for step {i + 1}:");
                Steps.Add(step);
            }
        }

        public void DisplayRecipe()
        {
            string recipeDetails = $"Recipe Name: {Name}\n\nIngredients:";
            double totalCalories = 0;

            for (int i = 0; i < Ingredients.Count; i++)
            {
                Ingredient ingredient = Ingredients[i];
                recipeDetails += $"\n  {i + 1}. {ingredient.Name}: {ingredient.Quantity} {ingredient.Unit}, Calories: {ingredient.Calories}, Food Group: {ingredient.FoodGroup}";
                totalCalories += ingredient.Calories;
            }

            recipeDetails += $"\n\nTotal Calories: {totalCalories}\n\nSteps:";

            for (int i = 0; i < Steps.Count; i++)
            {
                recipeDetails += $"\n  {i + 1}. {Steps[i]}";
            }

            if (totalCalories > 300)
            {
                CaloriesExceeded?.Invoke(Name, totalCalories);
            }

            MessageBox.Show(recipeDetails);
        }

        public void StoreOriginalQuantities()
        {
            for (int i = 0; i < Ingredients.Count; i++)
            {
                OriginalQuantities.Add(Ingredients[i].Quantity);
                OriginalUnits.Add(Ingredients[i].Unit);
            }
        }
    }
}
