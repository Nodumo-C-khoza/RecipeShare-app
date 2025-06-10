export interface Ingredient {
    id: number;
    name: string;
    amount: string;
    unit: string;
}

export interface Recipe {
    id: number;
    title: string;
    description: string;
    instructions: string;
    prepTimeMinutes: number;
    cookTimeMinutes: number;
    servings: number;
    imageUrl?: string;
    createdAt: Date;
    updatedAt: Date;
    dietaryTags: (string | { id: number; name: string; displayName: string; description: string; recipes: any[] })[];
    difficultyLevel: string;
    difficultyLevelId: number;
    ingredients: Ingredient[];
}

export interface CreateRecipe {
    title: string;
    description: string;
    instructions: string;
    prepTimeMinutes: number;
    cookTimeMinutes: number;
    servings: number;
    imageUrl?: string;
    dietaryTagIds: number[];
    ingredients: CreateIngredient[];
    difficultyLevel: string;
    difficultyLevelId: number;
}

export interface CreateIngredient {
    name: string;
    amount: string;
    unit: string;
}

export interface PaginatedResult<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
}

export interface RecipeListViewModel {
    id: number;
    title: string;
    description: string;
    prepTimeMinutes: number;
    cookTimeMinutes: number;
    servings: number;
    difficultyLevel: string;
    dietaryTags: string[];
    imageUrl?: string;
}

export interface RecipeDetailViewModel extends RecipeListViewModel {
    ingredients: IngredientViewModel[];
    instructions: string[];
    createdAt: Date;
    updatedAt: Date;
}

export interface IngredientViewModel {
    id: number;
    name: string;
    amount: string;
    unit: string;
}

export interface CreateRecipeViewModel {
    title: string;
    description: string;
    prepTimeMinutes: number;
    cookTimeMinutes: number;
    servings: number;
    difficultyLevel: string;
    dietaryTags: string[];
    ingredients: IngredientViewModel[];
    instructions: string[];
    imageUrl?: string;
}

export interface UpdateRecipeViewModel extends CreateRecipeViewModel {
    id: number;
}
