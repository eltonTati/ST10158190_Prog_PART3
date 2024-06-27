using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RecipeApp
{
    /// <summary>
    /// This is the cs file for my main, i have some functionalities of my program here 
    /// </summary>
    /// ---------------------------------------------------------------------------------------- //   
    public partial class MainWindow : Window
    {
        private ObservableCollection<Recipe> recipes;

        public MainWindow()
        {
            /// here i initialize componet 
            InitializeComponent();
            recipes = new ObservableCollection<Recipe>();
            RecipeListBox.ItemsSource = recipes;
        }
        //this is the method for adding the recipe
        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // here im validating and parsing inputs
                if (string.IsNullOrWhiteSpace(RecipeNameTextBox.Text))
                {
                    MessageBox.Show("Please enter a recipe name.");
                    return;
                }

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

                // here I'm creating a new Recipe instance with constructor parameters
                Recipe recipe = new Recipe(RecipeNameTextBox.Text, ingredientCount, stepCount);

                //  this is to Handle event if calories exceed
                recipe.CaloriesExceeded += (name, totalCalories) =>
                {
                    MessageBox.Show($"WARNING: The total calories of recipe '{name}' exceed 300. Total Calories: {totalCalories}");
                };

                // this is to perform recipe initialization (if these methods exist in your Recipe class)
                recipe.GetIngredients(ingredientCount);
                recipe.StoreOriginalQuantities();
                recipe.GetSteps(stepCount);

                // this is to Add recipe to ObservableCollection
                recipes.Add(recipe);

                // this part is just Clear input fields after successful addition
                RecipeNameTextBox.Clear();
                IngredientCountTextBox.Clear();
                StepCountTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }

        /// This is method is for applying filters 

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

            ///here im calling a method 
            UpdateRecipeListBox(filteredRecipes);
        }

        /// <summary>
        /// This is the method that has the funcionality to display the recipe selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        //this method is to update the recipe list according to the filter applied 
        private void UpdateRecipeListBox(List<Recipe> filteredRecipes = null)
        {
            //this checks is a certain recipe exist
            if (filteredRecipes == null && (recipes == null || recipes.Count == 0))
            {
                MessageBox.Show("No recipes available to display.");
                return;
            }

            if (filteredRecipes != null && filteredRecipes.Count == 0)
            {
                MessageBox.Show("No filtered recipes available to display.");
                return;
            }

            if (filteredRecipes != null)
            {
                RecipeListBox.ItemsSource = new ObservableCollection<Recipe>(filteredRecipes);
            }
            else
            {
                RecipeListBox.ItemsSource = recipes;
            }
            //this displays the name o te recipes 
            RecipeListBox.DisplayMemberPath = "Name";
        }

    }
}
