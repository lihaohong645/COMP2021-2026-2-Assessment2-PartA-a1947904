# Design

## 1. Saved recipe collection

The saved recipe feature uses a `HashSet<int>` named `_savedRecipes`.

Each value in the collection is the ID of a recipe saved by the user. A `HashSet<int>` is suitable because it automatically prevents duplicate recipe IDs from being stored.

The `Add` method returns `false` when the same recipe ID is already present. This allows `AddSavedRecipe` to reject duplicate saved recipes without performing a separate duplicate check.

The `Remove` method removes a recipe ID and returns whether the ID was present. The `Contains` method provides an efficient way to check whether a recipe is currently saved.

Adding, removing, and checking membership in a `HashSet<int>` have an average time complexity of O(1).

## 2. Integration

The Part B features are implemented inside the existing `RecipeManager` class and use the recipe catalogue created in Part A.

The title and ingredient search methods use LINQ to inspect recipes stored in the existing `Dictionary<int, Recipe>`. Title searches compare the search text with each recipe title. Ingredient searches inspect the ingredient list of each recipe.

The protein report uses the nutrition information stored in each recipe. Recipes without a protein value are excluded. The remaining recipes are sorted by protein amount in descending order, and the requested number of recipes is returned.

The saved recipe collection stores recipe IDs rather than complete recipe objects. Before a recipe is saved, `AddSavedRecipe` checks the existing recipe dictionary to confirm that the recipe ID is valid. When a recipe is removed from the catalogue, its ID is also removed from the saved recipe collection.

The Part B features reuse the existing recipe data and do not create duplicate recipe objects.

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | `Dictionary<int, Recipe>` | Average O(1) | A dictionary uses a hash-based key lookup. |
| Add a recipe | `Dictionary<int, Recipe>` | Average O(1) | The recipe ID is used as the dictionary key. |
| Remove a recipe | `Dictionary<int, Recipe>` | Average O(1) | Removing an item by key uses a hash lookup. |
| Traverse the shopping list | `List<string>` | O(n) | Each shopping list item may need to be visited. |
| Traverse the cooking plan | `LinkedList<int>` | O(n) | Each planned recipe may need to be visited. |
| Restore the last removed recipe | `Stack<int>` | O(1) | The item at the top of the stack is removed. |
| Complete the next instruction | `Queue<string>` | O(1) | The item at the front of the queue is removed. |
| LINQ title search | Recipe collection | O(n) | Each recipe title may need to be inspected. |
| LINQ ingredient search | Recipe and ingredient collections | O(n × m) | Each recipe and its ingredients may need to be inspected. |
| Sort recipes by protein | Recipe collection | O(n log n) | The matching recipes are sorted by protein amount. |
| Add a saved recipe | `HashSet<int>` | Average O(1) | Hash-based insertion is used and duplicates are rejected automatically. |
| Remove a saved recipe | `HashSet<int>` | Average O(1) | Hash-based removal is performed using the recipe ID. |
| Check whether a recipe is saved | `HashSet<int>` | Average O(1) | The `Contains` method performs a hash-based membership check. |
| Return all saved recipes | `HashSet<int>` and `List<int>` | O(s) | Each saved recipe ID is copied into a new list. |

In the complexity table, `n` is the number of recipes, `m` is the average number of ingredients in a recipe, and `s` is the number of saved recipes.