import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { AuthService } from './auth/auth.service';

describe('AppComponent', () => {
  let authServiceMock: Partial<AuthService>;
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;

  beforeEach(async () => {
    authServiceMock = {
      init: jasmine.createSpy('init')
    }

    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        AppComponent
      ],
      providers: [
        { provide: AuthService, useValue: authServiceMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the app', () => expect(component).toBeTruthy());

  it('should initialize authService during ngOnInit', () => {
    expect(authServiceMock.init).toHaveBeenCalled();
  });

  it('should render navigation menu', () => {
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('tt-nav-menu')).toBeTruthy();
  });

  it('should render router-outlet', () => {
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('router-outlet')).toBeTruthy();
  });
});
