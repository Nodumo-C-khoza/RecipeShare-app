import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RecipeDetailComponent } from './recipe-detail.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { RecipeService } from '../../services/recipe.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

describe('RecipeDetailComponent', () => {
  let component: RecipeDetailComponent;
  let fixture: ComponentFixture<RecipeDetailComponent>;
  let recipeService: jasmine.SpyObj<RecipeService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('RecipeService', ['getRecipe', 'deleteRecipe']);
    spy.getRecipe.and.returnValue(of({ id: 1, title: 'Test Recipe' }));
    spy.deleteRecipe.and.returnValue(of(void 0));

    await TestBed.configureTestingModule({
      imports: [
        RecipeDetailComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
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
    fixture = TestBed.createComponent(RecipeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
