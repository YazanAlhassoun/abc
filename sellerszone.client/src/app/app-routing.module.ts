import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { 
    path: '', 
    redirectTo: 'admin', 
    pathMatch: 'full' 
  }, // Default to the user project
  { 
    path: 'user', 
    loadChildren: () => import('../../projects/user/src/app/app.module').then(m => m.AppModule) 
  },
  { 
    path: 'admin', 
    loadChildren: () => import('../../projects/admin/src/app/app.module').then(m => m.AppModule) 
  },
  { 
    path: '**', 
    redirectTo: 'user' 
  }, // Wildcard route to handle invalid URLs
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
