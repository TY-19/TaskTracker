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
import { AssignmentViewFullComponent } from './assignments/assignment-view-full/assignment-view-full.component';
import { AssignmentsComponent } from './assignments/assignments.component';
import { AssignmentEditComponent } from './assignments/assignment-edit/assignment-edit.component';

const routes: Routes = [
  { path: '', component: BoardsComponent, pathMatch: 'full' },
  { path: 'login', component: AuthComponent },
  { path: 'boards', component: BoardsComponent, canActivate: [AuthGuard] },
  { path: 'boards/create', component: BoardCreateComponent, canActivate: [AuthGuard],
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'boards/:id', component: BoardDetailsComponent, canActivate: [AuthGuard] },
  { path: 'boards/:id/edit', component: BoardEditComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'boards/:boardId/stages', component: StagesComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'boards/:boardId/tasks/', component: AssignmentsComponent, canActivate: [AuthGuard] },
  { path: 'boards/:boardId/tasks/:taskId', component: AssignmentViewFullComponent, canActivate: [AuthGuard] },
  { path: 'boards/:boardId/tasks/:taskId/edit', component: AssignmentEditComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
