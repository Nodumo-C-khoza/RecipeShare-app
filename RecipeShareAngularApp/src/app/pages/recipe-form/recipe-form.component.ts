import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RecipeService } from '../../services/recipe.service';
import { Recipe, CreateRecipe, CreateIngredient } from '../../models/recipe';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import Swal from 'sweetalert2';

interface DietaryTag {
  id: number;
  name: string;
  displayName: string;
  description: string;
}

@Component({
  selector: 'app-recipe-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatChipsModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './recipe-form.component.html',
  styleUrl: './recipe-form.component.css'
})
export class RecipeFormComponent implements OnInit {
  recipe: Recipe | null = null;
  isEditMode = false;
  dietaryTags: DietaryTag[] = [];
  difficultyLevels: string[] = [];
  loading = false;

  private createNewIngredient(): CreateIngredient {
    return {
      name: '',
      amount: '',
      unit: ''
    };
  };

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private recipeService: RecipeService
  ) {}

  ngOnInit(): void {
    this.loadDietaryTags();
    this.loadDifficultyLevels();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.loadRecipe(Number(id));
    } else {
      this.recipe = {
        id: 0,
        title: '',
        description: '',
        instructions: '',
        prepTimeMinutes: 0,
        cookTimeMinutes: 0,
        servings: 0,
        imageUrl: '',
        createdAt: new Date(),
        updatedAt: new Date(),
        dietaryTags: [],
        difficultyLevel: '',
        difficultyLevelId: 1,
        ingredients: []
      };
    }
  }

  loadDietaryTags(): void {
    this.recipeService.getAvailableDietaryTags().subscribe({
      next: (tags) => {
        this.dietaryTags = tags;
      },
      error: (error) => {
        Swal.fire({
          icon: 'error',
          title: 'Error!',
          text: 'Failed to load dietary tags. Please try again.',
          confirmButtonColor: '#3085d6'
        });
        console.error('Error loading dietary tags:', error);
      }
    });
  }

  loadDifficultyLevels(): void {
    this.recipeService.getAvailableDifficultyLevels().subscribe({
      next: (levels) => {
        this.difficultyLevels = levels;
      },
      error: (error) => {
        Swal.fire({
          icon: 'error',
          title: 'Error!',
          text: 'Failed to load difficulty levels. Please try again.',
          confirmButtonColor: '#3085d6'
        });
        console.error('Error loading difficulty levels:', error);
      }
    });
  }

  loadRecipe(id: number): void {
    this.loading = true;
    this.recipeService.getRecipe(id).subscribe({
      next: (recipe) => {
        let dietaryTags: string[] = [];

        if (Array.isArray(recipe.dietaryTags) && recipe.dietaryTags.length > 0) {
          dietaryTags = recipe.dietaryTags.map(tag => {
            if (typeof tag === 'string') return tag;
            if (typeof tag === 'object' && tag !== null) return (tag as any).name;
            return '';
          }).filter(tag => tag !== '');
        }

        this.recipe = {
          ...recipe,
          dietaryTags: dietaryTags
        };
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        Swal.fire({
          icon: 'error',
          title: 'Error!',
          text: 'Failed to load recipe. Please try again.',
          confirmButtonColor: '#3085d6'
        });
        console.error('Error loading recipe:', error);
      }
    });
  }

  onSubmit(): void {
    if (!this.recipe) return;

    const dietaryTagIds = this.recipe.dietaryTags.map(tag => {
      if (typeof tag === 'string') {
        const matchingTag = this.dietaryTags.find(dt => dt.name === tag);
        if (!matchingTag) {
          console.warn(`Dietary tag "${tag}" not found in available tags`);
          return null;
        }
        return matchingTag.id;
      }
      return null;
    }).filter(id => id !== null) as number[];

    if (dietaryTagIds.length === 0) {
      Swal.fire({
        icon: 'warning',
        title: 'Validation Error',
        text: 'At least one dietary tag is required',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    if (dietaryTagIds.length !== this.recipe.dietaryTags.length) {
      Swal.fire({
        icon: 'warning',
        title: 'Validation Error',
        text: 'Some dietary tags could not be mapped. Please check your selections.',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    const ingredients = this.recipe.ingredients.map(ingredient => ({
      name: ingredient.name,
      amount: ingredient.amount,
      unit: ingredient.unit
    }));

    // Validate ingredients
    if (this.recipe.ingredients.length === 0) {
      Swal.fire({
        icon: 'warning',
        title: 'Validation Error',
        text: 'At least one ingredient is required',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    // Validate each ingredient's fields
    const invalidIngredients = this.recipe.ingredients.filter(ingredient =>
      !ingredient.name || !ingredient.amount || !ingredient.unit
    );

    if (invalidIngredients.length > 0) {
      Swal.fire({
        icon: 'warning',
        title: 'Validation Error',
        text: 'All ingredient fields (name, amount, unit) are required',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    if (!this.recipe.title || this.recipe.title.length < 3) {
      Swal.fire({
        icon: 'warning',
        title: 'Validation Error',
        text: 'Title must be at least 3 characters',
        confirmButtonColor: '#3085d6'
      });
      return;
    }
    if (!this.recipe.description) {
      Swal.fire({
        icon: 'warning',
        title: 'Validation Error',
        text: 'Description is required',
        confirmButtonColor: '#3085d6'
      });
      return;
    }
    if (!this.recipe.instructions) {
      Swal.fire({
        icon: 'warning',
        title: 'Validation Error',
        text: 'Instructions are required',
        confirmButtonColor: '#3085d6'
      });
      return;
    }
    if (!this.recipe.difficultyLevel) {
      Swal.fire({
        icon: 'warning',
        title: 'Validation Error',
        text: 'Difficulty level is required',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    const difficultyLevelId = this.recipe.difficultyLevel === 'Beginner' ? 1 :
                             this.recipe.difficultyLevel === 'Intermediate' ? 2 :
                             this.recipe.difficultyLevel === 'Advanced' ? 3 : 1;

    const recipeData: CreateRecipe = {
      title: this.recipe.title,
      description: this.recipe.description,
      instructions: this.recipe.instructions,
      prepTimeMinutes: this.recipe.prepTimeMinutes,
      cookTimeMinutes: this.recipe.cookTimeMinutes,
      servings: this.recipe.servings,
      imageUrl: this.recipe.imageUrl,
      dietaryTagIds: dietaryTagIds,
      ingredients: ingredients,
      difficultyLevel: this.recipe.difficultyLevel,
      difficultyLevelId: difficultyLevelId
    };

    if (this.isEditMode) {
      // Validate ingredients before update
      if (this.recipe.ingredients.length === 0) {
        Swal.fire({
          icon: 'warning',
          title: 'Validation Error',
          text: 'A recipe must have at least one ingredient.',
          confirmButtonColor: '#3085d6'
        });
        return;
      }

      this.recipeService.updateRecipe(this.recipe.id, recipeData).subscribe({
        next: () => {
          Swal.fire({
            icon: 'success',
            title: 'Success!',
            text: 'Recipe has been updated successfully.',
            confirmButtonColor: '#3085d6'
          }).then(() => {
            this.router.navigate(['/recipes']);
          });
        },
        error: (error) => {
          console.error('Error updating recipe:', error);
          Swal.fire({
            icon: 'error',
            title: 'Update Failed',
            text: 'Failed to update recipe. Please try again.',
            confirmButtonColor: '#3085d6'
          });
        }
      });
    } else {
      this.recipeService.createRecipe(recipeData).subscribe({
        next: () => {
          Swal.fire({
            icon: 'success',
            title: 'Success!',
            text: 'Recipe has been created successfully.',
            confirmButtonColor: '#3085d6'
          }).then(() => {
            setTimeout(() => {
              this.router.navigate(['/recipes']);
            }, 1000);
          });
        },
        error: (error) => {
          console.error('Error creating recipe:', error);
          Swal.fire({
            icon: 'error',
            title: 'Creation Failed',
            text: 'Failed to create recipe. Please try again.',
            confirmButtonColor: '#3085d6'
          });
        }
      });
    }
  }

  addIngredient(): void {
    if (!this.recipe) return;
    this.recipe.ingredients.push({
      id: 0,
      name: '',
      amount: '',
      unit: ''
    });
  }

  removeIngredient(index: number): void {
    if (!this.recipe) return;
    this.recipe.ingredients.splice(index, 1);
  }
}
