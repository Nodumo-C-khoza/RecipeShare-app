import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Recipe, CreateRecipe, PaginatedResult } from '../models/recipe';
import { tap, catchError } from 'rxjs/operators';

interface DietaryTag {
  id: number;
  name: string;
  displayName: string;
  description: string;
  recipes: any[];
}

@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  private apiUrl = 'http://localhost:5229/api/recipes';
  private recipeUpdateSubject = new BehaviorSubject<void>(undefined);
  public recipeUpdated$ = this.recipeUpdateSubject.asObservable();

  constructor(private http: HttpClient) { }

  // Method to notify components that recipes have been updated
  notifyRecipeUpdate(): void {
    this.recipeUpdateSubject.next();
  }

  getRecipes(
    pageNumber: number = 1,
    pageSize: number = 20,
    searchQuery?: string,
    tag?: string,
    difficulty?: string,
    maxTime?: number,
    quickRecipes?: boolean
  ): Observable<PaginatedResult<Recipe>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (searchQuery) {
      params = params.set('searchQuery', searchQuery);
    }
    if (tag) {
      params = params.set('tag', tag);
    }
    if (difficulty) {
      params = params.set('difficulty', difficulty);
    }
    if (maxTime) {
      params = params.set('maxTime', maxTime.toString());
    }
    if (quickRecipes) {
      params = params.set('quickRecipes', quickRecipes.toString());
    }

    return this.http.get<PaginatedResult<Recipe>>(`${this.apiUrl}/GetRecipes`, { params });
  }

  getRecipe(id: number): Observable<Recipe> {
    return this.http.get<Recipe>(`${this.apiUrl}/GetRecipe/${id}`);
  }

  createRecipe(recipe: CreateRecipe): Observable<Recipe> {
    return this.http.post<Recipe>(`${this.apiUrl}/CreateRecipe`, recipe).pipe(
      tap(response => {
        console.log('Recipe created successfully:', response);
        this.notifyRecipeUpdate();
      }),
      catchError(error => {
        console.error('Error creating recipe:', error);
        throw error;
      })
    );
  }

  updateRecipe(id: number, recipe: CreateRecipe): Observable<Recipe> {
    return this.http.put<Recipe>(`${this.apiUrl}/UpdateRecipe/${id}`, recipe).pipe(
      tap(response => {
        console.log('Recipe updated successfully:', response);
        this.notifyRecipeUpdate();
      }),
      catchError(error => {
        console.error(`Error updating recipe ${id}:`, error);
        throw error;
      })
    );
  }

  deleteRecipe(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/DeleteRecipe/${id}`).pipe(
      tap(() => {
        console.log('Recipe deleted successfully');
        this.notifyRecipeUpdate();
      }),
      catchError(error => {
        console.error(`Error deleting recipe ${id}:`, error);
        throw error;
      })
    );
  }

  getAvailableDietaryTags(): Observable<DietaryTag[]> {
    return this.http.get<DietaryTag[]>(`${this.apiUrl}/GetAvailableDietaryTags`);
  }

  getAvailableDifficultyLevels(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/GetAvailableDifficultyLevels`);
  }
}
