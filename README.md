# DishAndMovie System

DishAndMovie is a unique application that connects recipes and movies by their cultural origins, allowing users to discover dishes from a movie's country or find films related to a recipe's background.

---

## Features

### 🍽 Recipe Management
- *CRUD Operations*: Add, update, remove, and delete recipes.
- *Meal Planning*: Organize recipes into meal plans.
- *Ingredient Tracking*: Manage ingredients linked to recipes.

### 🎬 Movie Review System
- *CRUD Operations*: Add, update, remove and delete movies, genres, and reviews.
- *Genre Classification*: Associate movies with multiple genres.
- *User Reviews*: Collect and display reviews for movies.

### 🌍 Origin-Based Pairing (New Feature)
- *Cultural Tagging: Recipes and movies are linked by shared origins (e.g., *Italian, Indian).
- *Smart Recommendations*:  
  - Get movie suggestions for a recipe (e.g., Samosa → taare zameen par).  
  - Get recipe suggestions for a movie (e.g., Squid game → Kimchi).

---

## Database Relationships

### One-to-Many
- MealPlan → Recipes  
- Origin → Recipes  
- Origin → Movies  

### Many-to-Many
- Recipes ↔ Ingredients  
- Movies ↔ Genres 
- Recipes ↔ Movies  

---

## Technologies Used
- *Backend*: ASP.NET Core, C#
- *Database*: SQL Server, Entity Framework Core
- *Authentication*: ASP.NET Identity
- *Tools*: Swagger, Postman, Git

---

## GitHub Repository

To clone the repository:
```sh
git clone https://github.com/Jnlpatel/DishAndMovie
```
## To Run This Project

1. **Open Package Manager Console:**
   - Navigate to **Tools** > **NuGet Package Manager** > **Package Manager Console**.

2. **Run Database Migrations:**
   - In the console, type:
     ```
     update-database
     ```

3. **Add Data to Database:**
   - Go to **Tools** > **SQL Server Object Explorer** > **Database**.
   - Add records to the following tables:
     - **Origins**
     - **MealPlans**
     - **Recipes**
     - **Ingredients**
     - **Movies**
     - **Genres**

4. **Interact with API:**
   - Use API requests to interact with the **Origins**, **MealPlans**, **Recipes**, **Ingredients**, **Movies**, **Genre**, and **RecipeIngredients** tables.