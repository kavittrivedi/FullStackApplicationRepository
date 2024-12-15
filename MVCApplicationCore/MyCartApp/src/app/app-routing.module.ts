import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { SigninComponent } from './components/auth/signin/signin.component';
import { SignupComponent } from './components/auth/signup/signup.component';
import { IfExampleComponent } from './components/examples/if-example/if-example.component';
import { ForloopExampleComponent } from './components/examples/forloop-example/forloop-example.component';
import { CategoryListComponent } from './components/category/category-list/category-list.component';
import { ProductListComponent } from './components/product/product-list/product-list.component';
import { PipeExampleComponent } from './components/examples/pipe-example/pipe-example.component';
import { TemplateFormComponent } from './components/examples/template-form/template-form.component';
import { AddCategoryTfComponent } from './components/category/add-category-tf/add-category-tf.component';
import { CategoryDetailsComponent } from './components/category/category-details/category-details.component';
import { UpdateCategoryTfComponent } from './components/category/update-category-tf/update-category-tf.component';
import { AddProductComponent } from './components/product/add-product/add-product.component';
import { ReactiveFormComponent } from './components/examples/reactive-form/reactive-form.component';
import { AddProductRfComponent } from './components/product/add-product-rf/add-product-rf.component';
import { SignupsuccessComponent } from './components/auth/signupsuccess/signupsuccess.component';
import { authGuard } from './guards/auth.guard';
import { CategoriesProductsComponent } from './components/examples/categories-products/categories-products.component';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'privacy', component: PrivacyComponent },
  { path: 'categories', component: CategoryListComponent, canActivate: [authGuard] },
  { path: 'products', component: ProductListComponent, canActivate: [authGuard] },
  { path: 'signup', component: SignupComponent },
  { path: 'signin', component: SigninComponent },
  { path: 'ifexample', component: IfExampleComponent },
  { path: 'forloopexample', component: ForloopExampleComponent },
  { path: 'pipeexample', component: PipeExampleComponent },
  { path: 'templateform', component: TemplateFormComponent },
  { path: 'addcategory', component: AddCategoryTfComponent, canActivate: [authGuard] },
  { path: 'categorydetails/:id', component: CategoryDetailsComponent, canActivate: [authGuard] },
  { path: 'updatecategorydetails/:id', component: UpdateCategoryTfComponent, canActivate: [authGuard] },
  { path: 'addproduct', component: AddProductComponent, canActivate: [authGuard] },
  { path: 'reactiveform', component: ReactiveFormComponent },
  { path: 'addproductrf', component: AddProductRfComponent, canActivate: [authGuard] },
  { path: 'signupsuccess', component: SignupsuccessComponent },
  { path: 'categoriesproducts', component: CategoriesProductsComponent },
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
