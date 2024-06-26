import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthComponent } from './auth/auth.component';
import { AuthGuard } from './auth/auth.guard';
import { BoardsComponent } from './boards/boards.component';
import { BoardEditComponent } from './boards/board-edit/board-edit.component';
import { DefaultRolesNames } from './config/default-roles-names';
import { BoardCreateComponent } from './boards/board-create/board-create.component';
import { BoardDetailsComponent } from './boards/board-details/board-details.component';
import { StagesComponent } from './stages/stages.component';
import { AssignmentViewComponent } from './assignments/assignment-view/assignment-view.component';
import { AssignmentsComponent } from './assignments/assignments.component';
import { AssignmentEditComponent } from './assignments/assignment-edit/assignment-edit.component';
import { RegistrationComponent } from './auth/registration/registration.component';
import { EmployeesComponent } from './employees/employees.component';
import { AccountComponent } from './account/account.component';
import { ChangePasswordComponent } from './account/change-password/change-password.component';
import { UsersComponent } from './users/users.component';
import { UserCreateComponent } from './users/user-create/user-create.component';
import { UserDetailsComponent } from './users/user-details/user-details.component';
import { UserBoardsComponent } from './users/user-boards/user-boards.component';
import { UserEditComponent } from './users/user-edit/user-edit.component';
import { UserChangePasswordComponent } from './users/user-change-password/user-change-password.component';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full', canActivate: [AuthGuard] },
  { path: 'login', component: AuthComponent },
  { path: 'registration', component: RegistrationComponent },
  { path: 'profile', component: AccountComponent, canActivate: [AuthGuard] },
  { path: 'profile/changepassword', component: ChangePasswordComponent, canActivate: [AuthGuard] },
  { path: 'boards', component: BoardsComponent, canActivate: [AuthGuard] },
  { path: 'boards/create', component: BoardCreateComponent, canActivate: [AuthGuard],
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'boards/:id', component: BoardDetailsComponent, canActivate: [AuthGuard] },
  { path: 'boards/:id/edit', component: BoardEditComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'boards/:boardId/stages', component: StagesComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'boards/:boardId/employees', component: EmployeesComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'boards/:boardId/tasks', component: AssignmentsComponent, canActivate: [AuthGuard] },
  { path: 'boards/:boardId/tasks/create', component: AssignmentEditComponent, canActivate: [AuthGuard],
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'boards/:boardId/tasks/:taskId', component: AssignmentViewComponent, canActivate: [AuthGuard] },
  { path: 'boards/:boardId/tasks/:taskId/edit', component: AssignmentEditComponent, canActivate: [AuthGuard],
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'tasks', component: AssignmentsComponent, canActivate: [AuthGuard] },
  { path: 'users', component: UsersComponent, canActivate: [AuthGuard],
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE] } },
  { path: 'users/create', component: UserCreateComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE] } },
  { path: 'users/:userName', component: UserDetailsComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE] } },
  { path: 'users/:userName/edit', component: UserEditComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE] } },
  { path: 'users/:userName/changepassword', component: UserChangePasswordComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE] } },
  { path: 'users/:userName/boards', component: UserBoardsComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE] } },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
