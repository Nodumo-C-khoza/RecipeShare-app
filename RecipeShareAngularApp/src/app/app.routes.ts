import { Routes } from '@angular/router';
import { RecipeListComponent } from './pages/recipe-list/recipe-list.component';
import { RecipeDetailComponent } from './pages/recipe-detail/recipe-detail.component';
import { RecipeFormComponent } from './pages/recipe-form/recipe-form.component';

export const routes: Routes = [
  { path: '', redirectTo: '/recipes', pathMatch: 'full' },
  { path: 'recipes', component: RecipeListComponent },
  { path: 'recipes/new', component: RecipeFormComponent },
  { path: 'recipes/edit/:id', component: RecipeFormComponent },
  { path: 'recipes/:id', component: RecipeDetailComponent },
  { path: '**', redirectTo: '/recipes' } // Catch all route
];
