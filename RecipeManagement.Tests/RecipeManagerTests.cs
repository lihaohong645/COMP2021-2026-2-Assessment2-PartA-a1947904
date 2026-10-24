using System;
using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Unit tests for the Part A recipe management collections.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();

        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
        Assert.Equal("Recipe B", manager.FindRecipe(20)?.Title);
    }

    [Fact]
    public void Constructor_RejectsNullCollection()
    {
        Assert.Throws<ArgumentNullException>(
            () => new RecipeManager(null!));
    }

    [Fact]
    public void Constructor_RejectsInvalidRecipes()
    {
        var duplicateRecipes = new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A"
            },
            new Recipe
            {
                Id = 10,
                Title = "Recipe B"
            }
        };

        var invalidIdRecipes = new[]
        {
            new Recipe
            {
                Id = 0,
                Title = "Recipe A"
            }
        };

        var blankTitleRecipes = new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "   "
            }
        };

        Assert.Throws<ArgumentException>(
            () => new RecipeManager(duplicateRecipes));

        Assert.Throws<ArgumentException>(
            () => new RecipeManager(invalidIdRecipes));

        Assert.Throws<ArgumentException>(
            () => new RecipeManager(blankTitleRecipes));
    }

    [Fact]
    public void AddRecipe_AddsValidRecipeAndRejectsDuplicate()
    {
        var manager = CreateManager();

        var recipe = new Recipe
        {
            Id = 30,
            Title = "Recipe C"
        };

        Assert.True(manager.AddRecipe(recipe));
        Assert.Equal(3, manager.RecipeCount);

        Assert.Equal(
            "Recipe C",
            manager.FindRecipe(30)?.Title);

        Assert.False(manager.AddRecipe(recipe));
        Assert.Equal(3, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_RejectsNullRecipe()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentNullException>(
            () => manager.AddRecipe(null!));
    }

    [Theory]
    [InlineData(0, "Recipe C")]
    [InlineData(-1, "Recipe C")]
    [InlineData(30, "")]
    [InlineData(30, "   ")]
    public void AddRecipe_RejectsInvalidIdentity(
        int id,
        string title)
    {
        var manager = CreateManager();

        var invalidRecipe = new Recipe
        {
            Id = id,
            Title = title
        };

        Assert.False(manager.AddRecipe(invalidRecipe));
        Assert.Equal(2, manager.RecipeCount);
        Assert.Null(manager.FindRecipe(id));
    }

    [Fact]
    public void FindRecipe_ReturnsNullForMissingId()
    {
        var manager = CreateManager();

        Assert.Null(manager.FindRecipe(999));
    }

    [Fact]
    public void RemoveRecipe_RemovesExistingRecipeAndHandlesMissingId()
    {
        var manager = CreateManager();

        Assert.True(manager.RemoveRecipe(20));
        Assert.Equal(1, manager.RecipeCount);
        Assert.Null(manager.FindRecipe(20));

        Assert.False(manager.RemoveRecipe(999));
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact]
    public void RemoveRecipe_DoesNotRemoveRecipeInCookingPlan()
    {
        var manager = CreateManager();

        Assert.True(
            manager.AddRecipeToCookingPlan(10));

        Assert.False(manager.RemoveRecipe(10));

        Assert.NotNull(manager.FindRecipe(10));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void AddIngredientsToShoppingList_CopiesIngredientsInOrder()
    {
        var manager = CreateManager();

        int numberAdded =
            manager.AddIngredientsToShoppingList(10);

        Assert.Equal(2, numberAdded);
        Assert.Equal(2, manager.ShoppingItemCount);

        Assert.Equal(
            new[]
            {
                "1 apple",
                "2 eggs"
            },
            manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredientsToShoppingList_HandlesMissingRecipe()
    {
        var manager = CreateManager();

        int numberAdded =
            manager.AddIngredientsToShoppingList(999);

        Assert.Equal(0, numberAdded);
        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void ClearShoppingList_RemovesAllItems()
    {
        var manager = CreateManager();

        manager.AddIngredientsToShoppingList(10);
        manager.ClearShoppingList();

        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void CookingPlan_AppendsRecipesAndRejectsDuplicates()
    {
        var manager = CreateManager();

        Assert.True(
            manager.AddRecipeToCookingPlan(10));

        Assert.True(
            manager.AddRecipeToCookingPlan(20));

        Assert.False(
            manager.AddRecipeToCookingPlan(10));

        Assert.False(
            manager.AddRecipeToCookingPlan(999));

        Assert.Equal(2, manager.CookingPlanCount);

        Assert.Equal(
            new[] { 10, 20 },
            manager.GetCookingPlan());
    }

    [Fact]
    public void RemovingPlannedRecipe_PushesIdOntoStack()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);

        Assert.False(
            manager.RemoveRecipeFromCookingPlan(20));

        Assert.Equal(0, manager.RemovedRecipeCount);

        Assert.True(
            manager.RemoveRecipeFromCookingPlan(10));

        Assert.Equal(1, manager.RemovedRecipeCount);

        Assert.Equal(
            10,
            manager.PeekLastRemovedRecipe());

        Assert.Empty(manager.GetCookingPlan());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);

        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);

        Assert.Equal(
            20,
            manager.PeekLastRemovedRecipe());

        Assert.True(
            manager.RestoreLastRemovedRecipe());

        Assert.Equal(
            new[] { 20 },
            manager.GetCookingPlan());

        Assert.Equal(
            10,
            manager.PeekLastRemovedRecipe());

        Assert.True(
            manager.RestoreLastRemovedRecipe());

        Assert.Equal(
            new[] { 20, 10 },
            manager.GetCookingPlan());
    }

    [Fact]
    public void EmptyRemovedRecipeStack_IsHandledSafely()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekLastRemovedRecipe());
        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();

        Assert.True(manager.StartCooking(10));
        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.Equal(
            "First step",
            manager.PeekNextInstruction());

        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.Equal(
            "First step",
            manager.CompleteNextInstruction());

        Assert.Equal(1, manager.PendingInstructionCount);

        Assert.Equal(
            "Second step",
            manager.PeekNextInstruction());

        Assert.Equal(
            "Second step",
            manager.CompleteNextInstruction());

        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_RejectsMissingRecipeOrRecipeWithoutInstructions()
    {
        var manager = CreateManager();

        Assert.False(manager.StartCooking(999));
        Assert.False(manager.StartCooking(20));
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_ReplacesPreviousInstructionQueue()
    {
        var manager = CreateManager();

        manager.AddRecipe(new Recipe
        {
            Id = 30,
            Title = "Recipe C",
            Instructions = new()
            {
                "Only step"
            }
        });

        Assert.True(manager.StartCooking(10));
        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.True(manager.StartCooking(30));
        Assert.Equal(1, manager.PendingInstructionCount);

        Assert.Equal(
            "Only step",
            manager.PeekNextInstruction());
    }

    [Fact]
    public void EmptyInstructionQueue_IsHandledSafely()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void GetShoppingList_ReturnsIndependentSnapshot()
    {
        var manager = CreateManager();

        manager.AddIngredientsToShoppingList(10);

        IReadOnlyList<string> snapshot =
            manager.GetShoppingList();

        manager.ClearShoppingList();

        Assert.Equal(
            new[]
            {
                "1 apple",
                "2 eggs"
            },
            snapshot);

        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void GetCookingPlan_ReturnsIndependentSnapshot()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);

        IReadOnlyList<int> snapshot =
            manager.GetCookingPlan();

        manager.AddRecipeToCookingPlan(20);

        Assert.Equal(
            new[] { 10 },
            snapshot);

        Assert.Equal(
            new[] { 10, 20 },
            manager.GetCookingPlan());
    }

    [Fact]
    public void ShoppingList_AccumulatesIngredientsInRecipeOrder()
    {
        var manager = CreateManager();

        manager.AddRecipe(new Recipe
        {
            Id = 30,
            Title = "Recipe C",
            Ingredients = new()
            {
                "3 cups flour"
            }
        });

        Assert.Equal(
            2,
            manager.AddIngredientsToShoppingList(10));

        Assert.Equal(
            1,
            manager.AddIngredientsToShoppingList(30));

        Assert.Equal(
            new[]
            {
                "1 apple",
                "2 eggs",
                "3 cups flour"
            },
            manager.GetShoppingList());

        Assert.Equal(3, manager.ShoppingItemCount);
    }

    [Fact]
    public void FailedStartCooking_PreservesCurrentInstructionQueue()
    {
        var manager = CreateManager();

        Assert.True(manager.StartCooking(10));
        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.False(manager.StartCooking(999));

        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.Equal(
            "First step",
            manager.PeekNextInstruction());
    }

    [Fact]
    public void SearchByTitle_ReturnsCaseInsensitivePartialMatch()
    {
        var manager = CreateManager();

        IReadOnlyList<Recipe> results =
            manager.SearchByTitle("cIpE a");

        Recipe result = Assert.Single(results);

        Assert.Equal(10, result.Id);
        Assert.Equal("Recipe A", result.Title);
    }

    [Fact]
    public void SearchByTitle_ReturnsEmptyListWhenNothingMatches()
    {
        var manager = CreateManager();

        IReadOnlyList<Recipe> results =
            manager.SearchByTitle("chocolate");

        Assert.Empty(results);
    }

    [Fact]
    public void SearchByTitle_BlankTextReturnsAllRecipes()
    {
        var manager = CreateManager();

        IReadOnlyList<Recipe> results =
            manager.SearchByTitle(string.Empty);

        Assert.Equal(2, results.Count);

        Assert.Contains(
            results,
            recipe => recipe.Id == 10);

        Assert.Contains(
            results,
            recipe => recipe.Id == 20);
    }

    [Fact]
    public void SearchByIngredient_ReturnsCaseInsensitivePartialMatch()
    {
        var manager = CreateManager();

        IReadOnlyList<Recipe> results =
            manager.SearchByIngredient("APP");

        Recipe result = Assert.Single(results);

        Assert.Equal(10, result.Id);
        Assert.Equal("Recipe A", result.Title);
    }

    [Fact]
    public void SearchByIngredient_ReturnsEmptyListWhenNothingMatches()
    {
        var manager = CreateManager();

        IReadOnlyList<Recipe> results =
            manager.SearchByIngredient("chocolate");

        Assert.Empty(results);
    }

    [Fact]
    public void SearchByIngredient_BlankTextReturnsAllRecipes()
    {
        var manager = CreateManager();

        IReadOnlyList<Recipe> results =
            manager.SearchByIngredient(string.Empty);

        Assert.Equal(2, results.Count);

        Assert.Contains(
            results,
            recipe => recipe.Id == 10);

        Assert.Contains(
            results,
            recipe => recipe.Id == 20);
    }

    [Fact]
    public void SearchByIngredient_ReturnsRecipeOnlyOnceWhenMultipleIngredientsMatch()
    {
        var manager = new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 30,
                Title = "Apple Mix",
                Ingredients = new()
                {
                    "1 apple",
                    "200 ml apple juice"
                }
            }
        });

        IReadOnlyList<Recipe> results =
            manager.SearchByIngredient("apple");

        Recipe result = Assert.Single(results);

        Assert.Equal(30, result.Id);
    }

    [Fact]
    public void GetHighestProteinRecipes_ReturnsRequestedRecipesInDescendingOrder()
    {
        var manager = CreateProteinManager();

        IReadOnlyList<Recipe> results =
            manager.GetHighestProteinRecipes(2);

        Assert.Equal(2, results.Count);
        Assert.Equal(20, results[0].Id);
        Assert.Equal(30, results[1].Id);

        Assert.Equal(
            35.0,
            results[0].Nutrition?.ProteinG);

        Assert.Equal(
            20.0,
            results[1].Nutrition?.ProteinG);
    }

    [Fact]
    public void GetHighestProteinRecipes_ExcludesRecipesWithoutProteinValues()
    {
        var manager = CreateProteinManager();

        IReadOnlyList<Recipe> results =
            manager.GetHighestProteinRecipes(10);

        Assert.Equal(3, results.Count);

        Assert.DoesNotContain(
            results,
            recipe => recipe.Id == 40);

        Assert.DoesNotContain(
            results,
            recipe => recipe.Id == 50);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetHighestProteinRecipes_NonPositiveCountReturnsEmptyList(
        int count)
    {
        var manager = CreateProteinManager();

        IReadOnlyList<Recipe> results =
            manager.GetHighestProteinRecipes(count);

        Assert.Empty(results);
    }

    [Fact]
    public void GetHighestProteinRecipes_CountLargerThanAvailableReturnsAllValidRecipes()
    {
        var manager = CreateProteinManager();

        IReadOnlyList<Recipe> results =
            manager.GetHighestProteinRecipes(20);

        Assert.Equal(3, results.Count);
        Assert.Equal(20, results[0].Id);
        Assert.Equal(30, results[1].Id);
        Assert.Equal(10, results[2].Id);
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new()
                {
                    "1 apple",
                    "2 eggs"
                },
                Instructions = new()
                {
                    "First step",
                    "Second step"
                }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }

    private static RecipeManager CreateProteinManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Low Protein Recipe",
                Nutrition = new NutritionInfo
                {
                    ProteinG = 12.5
                }
            },
            new Recipe
            {
                Id = 20,
                Title = "High Protein Recipe",
                Nutrition = new NutritionInfo
                {
                    ProteinG = 35.0
                }
            },
            new Recipe
            {
                Id = 30,
                Title = "Medium Protein Recipe",
                Nutrition = new NutritionInfo
                {
                    ProteinG = 20.0
                }
            },
            new Recipe
            {
                Id = 40,
                Title = "Missing Nutrition Recipe",
                Nutrition = null
            },
            new Recipe
            {
                Id = 50,
                Title = "Missing Protein Recipe",
                Nutrition = new NutritionInfo
                {
                    ProteinG = null
                }
            }
        });
    }
}