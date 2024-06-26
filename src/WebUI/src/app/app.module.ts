import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, provideHttpClient, withInterceptors } from '@angular/common/http'
import { AngularMaterialModule } from './angular-material.module';

import { AppRoutingModule, routes } from './app-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { authInterceptor } from './auth/auth.interceptor';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthComponent } from './auth/auth.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { BoardsComponent } from './boards/boards.component';
import { BoardEditComponent } from './boards/board-edit/board-edit.component';
import { BoardCreateComponent } from './boards/board-create/board-create.component';
import { BoardDetailsComponent } from './boards/board-details/board-details.component';
import { StagesComponent } from './stages/stages.component';
import { StageCreateEditComponent } from './stages/stage-create-edit/stage-create-edit.component';
import { AssignmentsComponent } from './assignments/assignments.component';
import { AssignmentViewComponent } from './assignments/assignment-view/assignment-view.component';
import { AssignmentEditComponent } from './assignments/assignment-edit/assignment-edit.component';
import { RegistrationComponent } from './auth/registration/registration.component';
import { EmployeesComponent } from './employees/employees.component';
import { AccountComponent } from './account/account.component';
import { ChangePasswordComponent } from './account/change-password/change-password.component';
import { EmployeeAddComponent } from './employees/employee-add/employee-add.component';
import { UsersComponent } from './users/users.component';
import { UserCreateComponent } from './users/user-create/user-create.component';
import { UserDetailsComponent } from './users/user-details/user-details.component';
import { UserBoardsComponent } from './users/user-boards/user-boards.component';
import { UserBoardsAddComponent } from './users/user-boards-add/user-boards-add.component';
import { UserEditComponent } from './users/user-edit/user-edit.component';
import { UserChangePasswordComponent } from './users/user-change-password/user-change-password.component';
import { SubpartsComponent } from './subparts/subparts.component';
import { HomeComponent } from './home/home.component';
import { BoardDisplayOptionsComponent } from './boards/board-details/board-display-options/board-display-options.component';
import { AssignmentsSingleBoardModeComponent } from './assignments/components/assignments-single-board-mode/assignments-single-board-mode.component';
import { AssignmentsMultyBoardsModeComponent } from './assignments/components/assignments-multy-boards-mode/assignments-multy-boards-mode.component';
import { SidebarComponent } from './boards/board-details/sidebar/sidebar.component';
import { provideRouter } from '@angular/router';

@NgModule({
  declarations: [
    AppComponent,
    AuthComponent,
    NavMenuComponent,
    BoardsComponent,
    BoardEditComponent,
    BoardCreateComponent,
    BoardDetailsComponent,
    StagesComponent,
    StageCreateEditComponent,
    AssignmentsComponent,
    AssignmentViewComponent,
    AssignmentEditComponent,
    RegistrationComponent,
    EmployeesComponent,
    AccountComponent,
    ChangePasswordComponent,
    EmployeeAddComponent,
    UsersComponent,
    UserCreateComponent,
    UserDetailsComponent,
    UserBoardsComponent,
    UserBoardsAddComponent,
    UserEditComponent,
    UserChangePasswordComponent,
    SubpartsComponent,
    HomeComponent,
    BoardDisplayOptionsComponent,
    AssignmentsSingleBoardModeComponent,
    AssignmentsMultyBoardsModeComponent,
    SidebarComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule, 
    ReactiveFormsModule,
    AngularMaterialModule
  ],
  providers: [
    provideHttpClient(withInterceptors([
      authInterceptor
    ])),
    provideRouter(routes),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
