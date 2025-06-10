import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RecipeListComponent } from './recipe-list.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { RecipeService } from '../../services/recipe.service';
import { of } from 'rxjs';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

describe('RecipeListComponent', () => {
  let component: RecipeListComponent;
  let fixture: ComponentFixture<RecipeListComponent>;
  let recipeService: jasmine.SpyObj<RecipeService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('RecipeService', ['getRecipes', 'getAvailableDietaryTags', 'getAvailableDifficultyLevels', 'deleteRecipe']);
    spy.getRecipes.and.returnValue(of({ items: [], totalCount: 0 }));
    spy.getAvailableDietaryTags.and.returnValue(of([]));
    spy.getAvailableDifficultyLevels.and.returnValue(of([]));
    spy.deleteRecipe.and.returnValue(of(void 0));

    await TestBed.configureTestingModule({
      imports: [
        RecipeListComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [
        { provide: RecipeService, useValue: spy }
      ]
    })
    .compileComponents();
    
    recipeService = TestBed.inject(RecipeService) as jasmine.SpyObj<RecipeService>;
    fixture = TestBed.createComponent(RecipeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
