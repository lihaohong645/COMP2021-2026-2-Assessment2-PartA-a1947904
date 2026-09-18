# Recipe Management System

**Student Name:** Lihao Hong  
**Student ID:** 1947904  
**Course:** COMP2021 Software Development Practice  
**Assessment:** Assignment 2 Part A  
**Semester:** Semester 2, 2026  

## Project Overview

This project is a console recipe management application developed for COMP2021 Assignment 2 Part A.

The application loads recipe information from a JSON file. It allows the user to find recipes, manage a shopping list, organise a cooking plan, restore removed recipes, and process cooking instructions.

## Part A Implementation

The main Part A implementation is located in:

`RecipeManagement.Core/RecipeManager.cs`

The implementation provides the following functionality:

1. Load recipes into the recipe manager.
2. Find a recipe using its recipe ID.
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

## Collections Used

### Dictionary

`Dictionary<int, Recipe>` stores recipes using the recipe ID as the key.

This collection allows recipes to be located efficiently and prevents multiple recipes from using the same ID.

### List

`List<string>` stores the shopping list.

Ingredients are added in their original order. The complete shopping list can be displayed or cleared.

### LinkedList

`LinkedList<int>` stores recipe IDs in the cooking plan.

Recipes can be added to the end of the cooking plan, removed, displayed, and restored.

### Stack

`Stack<int>` stores the IDs of recipes removed from the cooking plan.

The most recently removed recipe is restored first. This follows Last In First Out behaviour.

### Queue

`Queue<string>` stores the cooking instructions for the currently selected recipe.

Instructions are completed in their original order. This follows First In First Out behaviour.

## Project Structure

### RecipeManagement.Core

Contains the recipe models, recipe loader, recipe manager interface, and Part A implementation.

### RecipeManagement.Application

Contains the console application, menu, and program entry point.

### RecipeManagement.Tests

Contains the xUnit automated tests for the Part A implementation.

### data

Contains the supplied `recipes.json` recipe dataset.

### Design.md

Contains the supplied design information for the application.

## Build the Project

Run the following command from the repository root:

```powershell
dotnet build
```

## Run the Tests

Run:

```powershell
dotnet test
```

The Part A test suite contains 19 tests.

The tests cover:

1. Constructor validation.
2. Recipe dictionary creation.
3. Finding recipes by ID.
4. Adding recipes.
5. Removing recipes.
6. Duplicate recipe handling.
7. Missing recipe handling.
8. Adding ingredients to the shopping list.
9. Clearing the shopping list.
10. Adding recipes to the cooking plan.
11. Removing recipes from the cooking plan.
12. Restoring recipes using stack order.
13. Starting a recipe.
14. Processing instructions using queue order.
15. Empty stack and queue behaviour.

All 19 tests pass.

## Run the Application

Run the application from the repository root:

```powershell
dotnet run --project RecipeManagement.Application -- data/recipes.json
```

The application loads the supplied recipe dataset and displays the console menu.

Menu options 1 to 14 provide the Part A functionality.

Menu options 15 to 21 belong to Part B and are outside the scope of this Part A submission.

## Manual Verification

The console application was manually tested using the supplied recipe dataset.

The following behaviours were verified:

1. Recipe ID 1 could be found and displayed.
2. Recipe ingredients could be added to the shopping list.
3. Shopping list items appeared in the correct order.
4. Recipes could be added to the cooking plan.
5. Recipes could be removed from the cooking plan.
6. The most recently removed recipe could be viewed.
7. Removed recipes could be restored in Last In First Out order.
8. Cooking instructions appeared in their original file order.
9. Completed cooking instructions were removed from the queue.

## Git Repository

The project source code and commit history are available at:

https://github.com/lihaohong645/COMP2021-2026-2-Assessment2-PartA-a1947904

## AI Acknowledgement

I used OpenAI ChatGPT as an assistive tool to help me interpret the assignment requirements, understand the required collection types, troubleshoot errors, and review implementation and testing ideas.

I reviewed the suggestions, tested the implementation in my own development environment, and verified the program through automated tests and manual console testing.