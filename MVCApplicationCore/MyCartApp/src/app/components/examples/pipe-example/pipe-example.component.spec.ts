import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PipeExampleComponent } from './pipe-example.component';
import { CapitalizePipe } from 'src/app/pipes/capitalize.pipe';



describe('PipeExampleComponent', () => {
  let component: PipeExampleComponent;
  let fixture: ComponentFixture<PipeExampleComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PipeExampleComponent, CapitalizePipe]
    });
    fixture = TestBed.createComponent(PipeExampleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
