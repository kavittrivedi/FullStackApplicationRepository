import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { SignupComponent } from './components/auth/signup/signup.component';
import { SigninComponent } from './components/auth/signin/signin.component';
import { IfExampleComponent } from './components/examples/if-example/if-example.component';
import { ForloopExampleComponent } from './components/examples/forloop-example/forloop-example.component';
import { CategoryListComponent } from './components/category/category-list/category-list.component';
import { ProductListComponent } from './components/product/product-list/product-list.component';
import { PipeExampleComponent } from './components/examples/pipe-example/pipe-example.component';
import { CapitalizePipe } from './pipes/capitalize.pipe';
import { TemplateFormComponent } from './components/examples/template-form/template-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AddCategoryTfComponent } from './components/category/add-category-tf/add-category-tf.component';
import { CategoryDetailsComponent } from './components/category/category-details/category-details.component';
import { UpdateCategoryTfComponent } from './components/category/update-category-tf/update-category-tf.component';
import { AddProductComponent } from './components/product/add-product/add-product.component';
import { ReactiveFormComponent } from './components/examples/reactive-form/reactive-form.component';
import { AddProductRfComponent } from './components/product/add-product-rf/add-product-rf.component';
import { SignupsuccessComponent } from './components/auth/signupsuccess/signupsuccess.component';
import { AuthService } from './services/auth.service';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { CategoriesProductsComponent } from './components/examples/categories-products/categories-products.component';
import { HeaderMenuComponent } from './components/shared/header-menu/header-menu.component';
import { FooterMenuComponent } from './components/shared/footer-menu/footer-menu.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    PrivacyComponent,
    CategoryListComponent,
    ProductListComponent,
    SignupComponent,
    SigninComponent,
    IfExampleComponent,
    ForloopExampleComponent,
    PipeExampleComponent,
    CapitalizePipe,
    TemplateFormComponent,
    AddCategoryTfComponent,
    CategoryDetailsComponent,
    UpdateCategoryTfComponent,
    AddProductComponent,
    ReactiveFormComponent,
    AddProductRfComponent,
    SignupsuccessComponent,
    CategoriesProductsComponent,
    HeaderMenuComponent,
    FooterMenuComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [AuthService,{provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule { }
