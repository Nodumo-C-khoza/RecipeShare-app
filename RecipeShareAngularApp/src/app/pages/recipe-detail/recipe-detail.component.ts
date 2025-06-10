import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RecipeService } from '../../services/recipe.service';
import { Recipe } from '../../models/recipe';

@Component({
  selector: 'app-recipe-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    HttpClientModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './recipe-detail.component.html',
  styleUrls: ['./recipe-detail.component.css']
})
export class RecipeDetailComponent implements OnInit {
  recipe: Recipe | null = null;
  loading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private recipeService: RecipeService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadRecipe(id);
    }
  }

  loadRecipe(id: number): void {
    this.loading = true;
    this.recipeService.getRecipe(id).subscribe({
      next: (recipe) => {
        this.recipe = recipe;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Error loading recipe';
        this.loading = false;
        console.error('Error loading recipe:', error);
      }
    });
  }

  deleteRecipe(): void {
    if (this.recipe && confirm('Are you sure you want to delete this recipe?')) {
      this.recipeService.deleteRecipe(this.recipe.id).subscribe({
        next: () => {
          this.goBack();
        },
        error: (error) => {
          this.error = 'Error deleting recipe';
          console.error('Error deleting recipe:', error);
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/recipes']);
  }
}
