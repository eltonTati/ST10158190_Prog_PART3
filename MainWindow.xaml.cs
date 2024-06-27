using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RecipeApp
{
    public partial class MainWindow : Window
    {
        private List<Recipe> recipes = new List<Recipe>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string recipeName = RecipeNameTextBox.Text;

                if (!int.TryParse(IngredientCountTextBox.Text, out int ingredientCount) || ingredientCount <= 0)
                {
                    MessageBox.Show("Please enter a valid positive integer for the number of ingredients.");
                    return;
                }

                if (!int.TryParse(StepCountTextBox.Text, out int stepCount) || stepCount <= 0)
                {
                    MessageBox.Show("Please enter a valid positive integer for the number of steps.");
                    return;
                }

                Recipe recipe = new Recipe(recipeName, ingredientCount, stepCount);

                recipe.CaloriesExceeded += (name, totalCalories) =>
                {
                    MessageBox.Show($"WARNING: The total calories of recipe '{name}' exceed 300. Total Calories: {totalCalories}");
                };

                recipe.GetIngredients(ingredientCount);
                recipe.StoreOriginalQuantities();
                recipe.GetSteps(stepCount);

                recipes.Add(recipe);

                RecipeListBox.Items.Add(recipe.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }
        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            string ingredientFilter = IngredientFilterTextBox.Text.ToLower();
            string foodGroupFilter = FoodGroupFilterTextBox.Text.ToLower();
            bool maxCaloriesFilterValid = double.TryParse(MaxCaloriesFilterTextBox.Text, out double maxCalories);

            var filteredRecipes = recipes.Where(r =>
                (string.IsNullOrEmpty(ingredientFilter) || r.Ingredients.Any(i => i.Name.ToLower().Contains(ingredientFilter))) &&
                (string.IsNullOrEmpty(foodGroupFilter) || r.Ingredients.Any(i => i.FoodGroup.ToLower().Contains(foodGroupFilter))) &&
                (!maxCaloriesFilterValid || r.Ingredients.Sum(i => i.Calories) <= maxCalories)
            ).ToList();

            UpdateRecipeListBox(filteredRecipes);
        }
        private void DisplayRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a recipe to display.");
                return;
            }

            var selectedRecipe = recipes[RecipeListBox.SelectedIndex];
            selectedRecipe.DisplayRecipe();
        }
        private void UpdateRecipeListBox(List<Recipe> filteredRecipes = null)
        {
            if (filteredRecipes == null && recipes == null)
            {
                MessageBox.Show("No recipes available to display.");
                return;
            }

            if (filteredRecipes != null && filteredRecipes.Count == 0)
            {
                MessageBox.Show("No filtered recipes available to display.");
            }

            // Clear the existing items
            RecipeListBox.Items.Clear();

            // Ensure that the property name "Name" exists in the Recipe class
            if (typeof(Recipe).GetProperty("Name") == null)
            {
                MessageBox.Show("The Recipe class does not contain a property named 'Name'.");
                return;
            }

            // Set the ItemsSource
            RecipeListBox.ItemsSource = filteredRecipes ?? recipes;
            RecipeListBox.DisplayMemberPath = "Name";
            RecipeListBox.Items.Refresh(); // Refresh the ListBox to ensure it displays the updated items
        }



    }
}
