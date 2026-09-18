using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implements the Part A recipe management collections.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    private readonly Dictionary<int, Recipe> _recipes = new();
    private readonly List<string> _shoppingList = new();
    private readonly LinkedList<int> _cookingPlan = new();
    private readonly Stack<int> _removedRecipes = new();
    private readonly Queue<string> _pendingInstructions = new();

    private static bool HasValidIdentity(Recipe recipe)
    {
        return recipe.Id > 0 &&
               !string.IsNullOrWhiteSpace(recipe.Title);
    }

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        ArgumentNullException.ThrowIfNull(recipes);

        foreach (Recipe recipe in recipes)
        {
            if (recipe is null ||
                !HasValidIdentity(recipe) ||
                !_recipes.TryAdd(recipe.Id, recipe))
            {
                throw new ArgumentException(
                    "Recipes must have a positive unique ID and a non-blank title.",
                    nameof(recipes));
            }
        }
    }

    public int RecipeCount => _recipes.Count;

    public int ShoppingItemCount => _shoppingList.Count;

    public int CookingPlanCount => _cookingPlan.Count;

    public int PendingInstructionCount => _pendingInstructions.Count;

    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        if (!HasValidIdentity(recipe))
        {
            return false;
        }

        return _recipes.TryAdd(recipe.Id, recipe);
    }

    public Recipe? FindRecipe(int recipeId)
    {
        return _recipes.TryGetValue(recipeId, out Recipe? recipe)
            ? recipe
            : null;
    }

    public bool RemoveRecipe(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId) ||
            _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        return _recipes.Remove(recipeId);
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe is null)
        {
            return 0;
        }

        foreach (string ingredient in recipe.Ingredients)
        {
            _shoppingList.Add(ingredient);
        }

        return recipe.Ingredients.Count;
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        return new List<string>(_shoppingList);
    }

    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId) ||
            _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if (!_cookingPlan.Remove(recipeId))
        {
            return false;
        }

        _removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
        {
            return false;
        }

        int recipeId = _removedRecipes.Pop();

        if (!_recipes.ContainsKey(recipeId) ||
            _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        return _removedRecipes.Count == 0
            ? null
            : _removedRecipes.Peek();
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        return new List<int>(_cookingPlan);
    }

    public bool StartCooking(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe is null ||
            recipe.Instructions.Count == 0)
        {
            return false;
        }

        _pendingInstructions.Clear();

        foreach (string instruction in recipe.Instructions)
        {
            _pendingInstructions.Enqueue(instruction);
        }

        return true;
    }

    public string? PeekNextInstruction()
    {
        return _pendingInstructions.Count == 0
            ? null
            : _pendingInstructions.Peek();
    }

    public string? CompleteNextInstruction()
    {
        return _pendingInstructions.Count == 0
            ? null
            : _pendingInstructions.Dequeue();
    }

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException(
            "Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException(
            "Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException(
            "Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException(
            "Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException(
            "Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException(
            "Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException(
            "Part B: implement GetSavedRecipes.");
}