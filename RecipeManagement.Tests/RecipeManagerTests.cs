using System;
using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Unit tests for the Part A recipe-management collections.
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
            new Recipe { Id = 10, Title = "Recipe A" },
            new Recipe { Id = 10, Title = "Recipe B" }
        };

        var invalidIdRecipes = new[]
        {
            new Recipe { Id = 0, Title = "Recipe A" }
        };

        var blankTitleRecipes = new[]
        {
            new Recipe { Id = 10, Title = "   " }
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
        Assert.Equal("Recipe C", manager.FindRecipe(30)?.Title);

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
    public void AddRecipe_RejectsInvalidIdentity(int id, string title)
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

        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.False(manager.RemoveRecipe(10));

        Assert.NotNull(manager.FindRecipe(10));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void AddIngredientsToShoppingList_CopiesIngredientsInOrder()
    {
        var manager = CreateManager();

        int numberAdded = manager.AddIngredientsToShoppingList(10);

        Assert.Equal(2, numberAdded);
        Assert.Equal(2, manager.ShoppingItemCount);

        Assert.Equal(
            new[] { "1 apple", "2 eggs" },
            manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredientsToShoppingList_HandlesMissingRecipe()
    {
        var manager = CreateManager();

        int numberAdded = manager.AddIngredientsToShoppingList(999);

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

        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.True(manager.AddRecipeToCookingPlan(20));
        Assert.False(manager.AddRecipeToCookingPlan(10));
        Assert.False(manager.AddRecipeToCookingPlan(999));

        Assert.Equal(2, manager.CookingPlanCount);
        Assert.Equal(new[] { 10, 20 }, manager.GetCookingPlan());
    }

    [Fact]
    public void RemovingPlannedRecipe_PushesIdOntoStack()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);

        Assert.False(manager.RemoveRecipeFromCookingPlan(20));
        Assert.Equal(0, manager.RemovedRecipeCount);

        Assert.True(manager.RemoveRecipeFromCookingPlan(10));
        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(10, manager.PeekLastRemovedRecipe());
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

        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());

        Assert.Equal(10, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20, 10 }, manager.GetCookingPlan());
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

        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal(1, manager.PendingInstructionCount);

        Assert.Equal("Second step", manager.PeekNextInstruction());
        Assert.Equal("Second step", manager.CompleteNextInstruction());

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
            Instructions = new() { "Only step" }
        });

        Assert.True(manager.StartCooking(10));
        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.True(manager.StartCooking(30));
        Assert.Equal(1, manager.PendingInstructionCount);
        Assert.Equal("Only step", manager.PeekNextInstruction());
    }

    [Fact]
    public void EmptyInstructionQueue_IsHandledSafely()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
        Assert.Equal(0, manager.PendingInstructionCount);
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
}