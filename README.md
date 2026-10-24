# Recipe Management System

**Student Name:** Lihao Hong  
**Student ID:** 1947904  
**Course:** COMP2021 Software Development Practice  
**Assessment:** Assignment 2 Parts A and B  
**Semester:** Semester 2, 2026  

## Project Overview

This project is a console recipe management application developed for COMP2021 Assignment 2.

The application loads recipe information from a JSON file and provides features for managing recipes, shopping lists, cooking plans, cooking instructions, searches, nutrition reports, and saved recipes.

Part A focuses on the use of core collection types.

Part B extends the existing system with LINQ searching, a highest-protein recipe report, and a saved recipe collection.

## Features

### Part A Features

The Part A implementation provides the following functionality:

1. Load recipes from a JSON file.
2. Find a recipe by its ID.
3. Add a new recipe.
4. Remove an existing recipe.
5. Add recipe ingredients to the shopping list.
6. Display the shopping list.
7. Clear the shopping list.
8. Add recipes to the cooking plan.
9. Remove recipes from the cooking plan.
10. View the most recently removed recipe.
11. Restore the most recently removed recipe.
12. Start cooking a selected recipe.
13. View the next cooking instruction.
14. Complete cooking instructions in their original order.

### Part B Features

The Part B implementation provides the following functionality:

1. Search for recipes by title.
2. Perform case-insensitive partial title matching.
3. Search for recipes by ingredient.
4. Perform case-insensitive partial ingredient matching.
5. Display recipes with the highest protein values.
6. Sort protein results in descending order.
7. Exclude recipes without protein information.
8. Save an existing recipe.
9. Prevent duplicate saved recipes.
10. Remove a saved recipe.
11. Check whether a recipe is saved.
12. Display all saved recipe IDs.
13. Remove a recipe ID from the saved collection when the recipe is deleted.

## Collections Used

### Dictionary

A `Dictionary<int, Recipe>` stores recipes using the recipe ID as the key.

This collection supports efficient recipe lookup, addition, and removal. It also prevents multiple recipes from using the same ID.

Average lookup, insertion, and removal complexity is O(1).

### List

A `List<string>` stores the shopping list.

Ingredients are added in their original order. Ingredients from multiple recipes accumulate until the shopping list is cleared.

### LinkedList

A `LinkedList<int>` stores recipe IDs in the cooking plan.

Recipes can be added to the end of the plan, removed, displayed, and restored.

### Stack

A `Stack<int>` stores IDs removed from the cooking plan.

The most recently removed recipe is restored first. This follows Last In First Out behaviour.

### Queue

A `Queue<string>` stores cooking instructions for the selected recipe.

Instructions are completed in their original order. This follows First In First Out behaviour.

### HashSet

A `HashSet<int>` stores saved recipe IDs.

The collection automatically prevents duplicate IDs. It supports average O(1) insertion, removal, and membership checks.

## LINQ Usage

LINQ is used for the Part B searching and reporting features.

### Title Search

`SearchByTitle` searches all recipe titles using case-insensitive partial matching.

A blank search value returns all recipes.

### Ingredient Search

`SearchByIngredient` examines the ingredient list of each recipe using case-insensitive partial matching.

Each recipe appears only once, even when several of its ingredients match the search value.

A blank search value returns all recipes.

### Highest Protein Report

`GetHighestProteinRecipes` excludes recipes without protein information, sorts valid recipes by protein amount in descending order, and returns the requested number of results.

A count of zero or a negative count returns an empty list.

## Saved Recipe Behaviour

The saved recipe feature stores recipe IDs in a `HashSet<int>`.

A recipe can be saved only when its ID exists in the main recipe dictionary.

Attempting to save the same recipe more than once returns `false`.

Removing an ID that has not been saved also returns `false`.

`GetSavedRecipes` returns a new list containing the saved IDs. Changes made to the saved collection after the method call do not affect the returned snapshot.

When a recipe is removed from the main recipe catalogue, its ID is also removed from the saved recipe collection.

## Validation and Error Handling

The recipe manager validates recipes before adding them to the dictionary.

A valid recipe must have:

1. A positive recipe ID.
2. A non-blank title.
3. A unique recipe ID.

The constructor rejects:

1. A null recipe collection.
2. Duplicate recipe IDs.
3. Non-positive recipe IDs.
4. Blank recipe titles.

Operations involving missing recipe IDs return a safe result without terminating the program.

## Project Structure

### RecipeManagement.Core

Contains:

- Recipe model classes
- Nutrition model
- JSON recipe loader
- Recipe manager interface
- Recipe manager implementation
- Part A collection features
- Part B search, report, and saved recipe features

### RecipeManagement.Application

Contains:

- Console menu
- Program entry point
- User input handling
- Recipe output formatting

### RecipeManagement.Tests

Contains the xUnit automated tests for Parts A and B.

### data

Contains the supplied `recipes.json` dataset.

### Design.md

Explains the saved recipe collection, Part A and Part B integration, and the expected time complexity of the main operations.

## Build the Project

Open a terminal in the repository root and run:

```powershell
dotnet build
```

A successful build should complete with no errors.

## Run the Tests

Run:

```powershell
dotnet test
```

The test project contains 40 test methods.

Parameterized tests execute multiple input cases, producing a total of 44 executed xUnit test cases.

The current expected result is:

```text
Total tests: 44
Passed: 44
Failed: 0
Skipped: 0
```

The automated tests cover:

1. Constructor validation.
2. Recipe dictionary creation.
3. Invalid recipe identity validation.
4. Recipe lookup by ID.
5. Adding valid recipes.
6. Rejecting duplicate recipe IDs.
7. Rejecting null recipes.
8. Removing recipes.
9. Protecting recipes in the cooking plan.
10. Adding ingredients to the shopping list.
11. Preserving ingredient order.
12. Accumulating shopping list items.
13. Clearing the shopping list.
14. Returning independent shopping list snapshots.
15. Adding recipes to the cooking plan.
16. Rejecting duplicate cooking plan entries.
17. Removing recipes from the cooking plan.
18. Restoring recipes in Last In First Out order.
19. Returning independent cooking plan snapshots.
20. Starting a recipe.
21. Processing instructions in First In First Out order.
22. Replacing the instruction queue.
23. Preserving the instruction queue after an invalid start attempt.
24. Handling empty stacks and queues.
25. Searching by title.
26. Case-insensitive title matching.
27. Searching by ingredient.
28. Case-insensitive ingredient matching.
29. Preventing duplicate ingredient search results.
30. Sorting recipes by protein.
31. Limiting protein report results.
32. Excluding missing nutrition values.
33. Handling non-positive report counts.
34. Saving existing recipes.
35. Rejecting missing recipe IDs.
36. Preventing duplicate saved recipes.
37. Removing saved recipes.
38. Checking saved recipe membership.
39. Returning independent saved recipe snapshots.
40. Removing deleted recipes from the saved collection.

All 44 executed test cases pass.

## Run the Application

Run the application from the repository root:

```powershell
dotnet run --project RecipeManagement.Application -- data/recipes.json
```

The application loads the supplied recipe dataset and displays the console menu.

Menu options 1 to 14 provide the Part A functionality.

Menu options 15 to 21 provide the Part B functionality.

## Console Menu

The application provides the following options:

1. Find and display recipe by ID.
2. Add a recipe.
3. Remove a recipe.
4. Add recipe ingredients to the shopping list.
5. Display the shopping list.
6. Clear the shopping list.
7. Add a recipe to the cooking plan.
8. Remove a recipe from the cooking plan.
9. View the last removed recipe.
10. Restore the last removed recipe.
11. Display the cooking plan.
12. Start cooking a recipe.
13. View the next cooking instruction.
14. Complete the next cooking instruction.
15. Search recipes by title.
16. Search recipes by ingredient.
17. Display highest-protein recipes.
18. Save a recipe.
19. Remove a saved recipe.
20. Display saved recipes.
21. Check whether a recipe is saved.
0. Exit the application.

## Manual Verification

The console application was manually tested with the supplied recipe dataset.

The following behaviours were verified:

1. Recipes can be located by ID.
2. Recipe ingredients can be added to the shopping list.
3. Shopping list items appear in the correct order.
4. The shopping list can be cleared.
5. Recipes can be added to and removed from the cooking plan.
6. Removed recipes can be restored in Last In First Out order.
7. Cooking instructions are processed in First In First Out order.
8. Title searches return matching recipes.
9. Ingredient searches return matching recipes.
10. Searches are case-insensitive.
11. Protein reports are ordered from highest to lowest protein.
12. Recipes can be saved and removed from the saved collection.
13. Duplicate saved recipes are rejected.
14. Missing recipe IDs are handled safely.
15. Empty collection operations do not terminate the application.

## Git Repository

The project source code and commit history are available at:

https://github.com/lihaohong645/COMP2021-2026-2-Assessment2-PartA-a1947904

Part A and Part B are maintained in the same repository.

## AI Acknowledgement

I used OpenAI ChatGPT as an assistive tool to help me interpret the assignment requirements, understand the required collection types, troubleshoot errors, and review implementation and testing ideas.

I reviewed the suggestions, tested the implementation in my own development environment, and verified the program through automated tests and manual console testing.