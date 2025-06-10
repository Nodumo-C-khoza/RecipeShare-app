import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RecipeFormComponent } from './recipe-form.component';
import { ActivatedRoute, Router } from '@angular/router';
import { RecipeService } from '../../services/recipe.service';
import { of } from 'rxjs';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';

describe('RecipeFormComponent', () => {
  let component: RecipeFormComponent;
  let fixture: ComponentFixture<RecipeFormComponent>;
  let recipeService: jasmine.SpyObj<RecipeService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('RecipeService', [
      'getRecipe',
      'createRecipe',
      'updateRecipe',
      'getAvailableDietaryTags',
      'getAvailableDifficultyLevels'
    ]);

    // Setup default return values for the spy methods
    spy.getAvailableDietaryTags.and.returnValue(of([]));
    spy.getAvailableDifficultyLevels.and.returnValue(of(['Beginner', 'Intermediate', 'Advanced']));
    spy.getRecipe.and.returnValue(of({
      id: 1,
      title: 'Test Recipe',
      description: 'Test Description',
      instructions: 'Test Instructions',
      prepTimeMinutes: 30,
      cookTimeMinutes: 60,
      servings: 4,
      imageUrl: 'test.jpg',
      createdAt: new Date(),
      updatedAt: new Date(),
      dietaryTags: [],
      difficultyLevel: 'Beginner',
      difficultyLevelId: 1,
      ingredients: []
    }));

    await TestBed.configureTestingModule({
      imports: [
        RecipeFormComponent,
        NoopAnimationsModule,
        RouterTestingModule
      ],
      providers: [
        { provide: RecipeService, useValue: spy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => '1'
              }
            }
          }
        }
      ]
    })
    .compileComponents();

    recipeService = TestBed.inject(RecipeService) as jasmine.SpyObj<RecipeService>;
    fixture = TestBed.createComponent(RecipeFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
