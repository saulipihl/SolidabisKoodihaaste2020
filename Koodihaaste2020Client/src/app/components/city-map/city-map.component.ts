import { Component, OnInit } from '@angular/core';
import { BusDataFetcher } from '../../services/busDataFetcher';
import { BusStop } from '../../models/busStop'

/**
 * This component is the heart of the application. It renders the map and handles all things related to it.
 * It handles e.g. dragging controls and calculating the positions for the bus stops.
 * Map image taken from https://depositphotos.com/322561700/stock-illustration-city-navigation-map-flat-plan.html
 */
@Component({
  selector: 'city-map',
  templateUrl: './city-map.component.html',
  styleUrls: ['./city-map.component.scss']
})
export class CityMap implements OnInit {
  constructor(
    private busDataFetcher: BusDataFetcher
  ) {}
  showUserZoomError: boolean = false;
  loading: boolean = true;
  busStops: BusStop[];
  isDown: boolean = false;
  mapDraggableDiv: any;
  mapContainerDiv: any;
  mapImageDiv: any;

  ngOnInit() {
    this.loadBusStops();
  }

  private setMapHTMLElements() {
    this.mapImageDiv = document.getElementById('map-image');
    this.mapDraggableDiv = document.getElementById("map-draggable-div");
    this.mapContainerDiv = document.getElementById("map-container-div");
  }

  // Add necessary event listeners to drag the map
  private addEventListeners() {
    this.mapContainerDiv.addEventListener('mousedown', () => this.setIsDown(true), true); // when mouse is down, toggle boolean isDown
    document.addEventListener('mouseup', () => this.setIsDown(false), true); // when mouse is up, toggle boolean isDown
    document.addEventListener('mousemove', e => this.moveMap(e), true); // Move the map-draggable-div on mouse move
  }

  // Load bus stop data from backend
  private loadBusStops() {
    this.busDataFetcher.getAllBusStops().subscribe((stops: BusStop[]) => {
      this.busStops = stops;
      this.loading = false;
      setTimeout(() => {  // Add a small time out so that all is rendered after setting the loading flag
        this.setMapHTMLElements();
        this.addEventListeners();
      }, 10)
      
    });
  }

  // Move the map on mouse move
  moveMap(e: MouseEvent) {
    e.preventDefault();
    if (this.isDown) { // Only move the map if mouse key is pressed down
      let deltaX = e.movementX;
      let deltaY = e.movementY;
      let rect = this.mapDraggableDiv.getBoundingClientRect();

      this.handleMapXDrag(deltaX, rect);
      this.handleMapYDrag(deltaY, rect);
    }
  }

  private handleMapXDrag(deltaX: number, rect: any) {
    if (deltaX !== 0) { // If moving on x-axis
      let newCoordinateX = rect.x + deltaX;
      if (newCoordinateX < 0) {
        if (newCoordinateX > -this.mapImageDiv.offsetWidth + window.innerWidth) {
          this.mapDraggableDiv.style.left = newCoordinateX + 'px';
        }
        else { // If trying to move image out of screen from right, prevent it
          this.mapDraggableDiv.style.left = -this.mapImageDiv.offsetWidth + window.innerWidth - 1 + 'px';
        }
      }
      else { // If trying to move image out of screen from left, prevent it
        this.mapDraggableDiv.style.left = '0px';
      }
    }
  }

  private handleMapYDrag(deltaY: number, rect: any) {
    if (deltaY !== 0) { // If moving on y-axis
      let newCoordinateY = rect.y + deltaY;
      if (newCoordinateY < 0) {
        if (newCoordinateY > -this.mapImageDiv.offsetHeight + window.innerHeight) {
          this.mapDraggableDiv.style.top = newCoordinateY + 'px';
        }
        else { // If trying to move image out of screen from bottom, prevent it
          this.mapDraggableDiv.style.top = -this.mapImageDiv.offsetHeight + window.innerHeight - 1 + 'px';
        }
      }
      else { // If trying to move image out of screen from top, prevent it
        this.mapDraggableDiv.style.top = '0px';
      }
    }
  }

  // Sets the value for isDown
  setIsDown(isDown: boolean): void {
    this.isDown = isDown;
  }

  // Handle zoom out button click
  zoomOut() {
    let { image, width, height } = this.GetImageAndHeightWidth();
    let { originalWidth, originalHeight } = this.GetImageOriginalWidthHeight(width, height);
    let newWidth = originalWidth*0.85; // change the size by 15%

    // Draggable div left value is used to check if zoom is going out of bounds on the right side of the screen
    let draggableDivLeft = this.GetDraggableDivLeftValue();

    if(newWidth + draggableDivLeft < window.innerWidth) { // Ensure that zoom out doesn't go out of bounds
      // Ideally here perform some width/height/left/top calculations to set the zoom out correctly, but time ran out to implement.
      // TODO: Instead of error message, handle the out of bounds zoom out by calculating it correctly.
      this.showUserZoomError=true;
      setTimeout(() => {
        this.showUserZoomError=false;
      },3000);
    } else {
      let calculatedHeight = this.GetImageNewHeight(originalWidth, originalHeight, newWidth); // Height needs to be calculated according to width change

      // Set the image width and height
      image.style.width = newWidth + "px";
      image.style.height = calculatedHeight + "px";

      this.moveBusStopsOnZoom(false, originalWidth, newWidth, originalHeight, calculatedHeight);
    }
  }

  private GetDraggableDivLeftValue() {
    let draggableDivLeftString = this.mapDraggableDiv.style.left;
    let draggableDivLeft = draggableDivLeftString
      ? parseFloat(draggableDivLeftString.substring(0, draggableDivLeftString.length - 2))
      : 0;
    return draggableDivLeft;
  }

  zoomIn() {
    let { image, width, height } = this.GetImageAndHeightWidth();
    let { originalWidth, originalHeight } = this.GetImageOriginalWidthHeight(width, height);
    let newWidth = originalWidth*1.15; // Increase width by 15%
    let calculatedHeight = this.GetImageNewHeight(originalWidth, originalHeight, newWidth); // Height needs to be calculated according to width change

    // Set the image width and height
    image.style.width = newWidth + "px";
    image.style.height = calculatedHeight + "px";

    this.moveBusStopsOnZoom(true, originalWidth, newWidth, originalHeight, calculatedHeight);
  }

  // Bus stops are dynamically set on the map and need to be moved when the image width changes
  private moveBusStopsOnZoom(goingCloser: boolean, mapOriginalWidth: number, mapNewWidth: number, mapOriginalHeight: number, mapCalculatedHeight: number) {
    let busStopDivs = document.getElementsByClassName('bus-stop');
    Array.prototype.forEach.call(busStopDivs, function (busStop: HTMLElement) {
      // Get bus stop new width/height
      let busStopOriginalWidth = window.getComputedStyle(busStop).getPropertyValue('width');
      let busStopToMapWidthRatio = parseFloat(busStopOriginalWidth.substring(0, busStopOriginalWidth.length - 2)) / mapOriginalWidth;
      let busStopNewHeightWidth = busStopToMapWidthRatio * mapNewWidth;

      // Calculate ratios for the top and left values. They're used to calculate the new left/top values
      let busStopLeftFloat = parseFloat(busStop.style.left.substring(0, busStop.style.left.length - 2));
      let ratio = busStopLeftFloat / mapOriginalWidth;
      let busStopTopFloat = parseFloat(busStop.style.top.substring(0, busStop.style.top.length - 2));
      let topRatio = busStopTopFloat / mapOriginalHeight;
      
      // Set the bus stop width and height
      busStop.style.width = busStopNewHeightWidth + "px";
      busStop.style.height = busStopNewHeightWidth + "px";

      // Depending on if going zooming or going farther, set the left/top
      if(goingCloser) {
        busStop.style.left = busStopLeftFloat + ratio * (mapNewWidth - mapOriginalWidth) + "px";
        busStop.style.top = busStopTopFloat + topRatio * (mapCalculatedHeight - mapOriginalHeight) + "px";
      } else {
        busStop.style.left = busStopLeftFloat - ratio * (mapOriginalWidth - mapNewWidth) + "px";
        busStop.style.top = busStopTopFloat - topRatio * (mapOriginalHeight - mapCalculatedHeight) + "px";
      }
    });
  }

  // New height of the image is calculated using the logic that the ratio of width/height stays the same
  private GetImageNewHeight(originalWidth: number, originalHeight: number, newWidth: number) {
    let imageRatio = originalWidth / originalHeight;
    let calculatedHeight = newWidth / imageRatio;
    return calculatedHeight;
  }

  private GetImageOriginalWidthHeight(width: string, height: string) {
    let originalWidth = parseFloat(width.substring(0, width.length - 2));
    let originalHeight = parseFloat(height.substring(0, height.length - 2));
    return { originalWidth, originalHeight };
  }

  private GetImageAndHeightWidth() {
    let image = this.mapImageDiv;
    let width = window.getComputedStyle(this.mapImageDiv).getPropertyValue('width');
    let height = window.getComputedStyle(this.mapImageDiv).getPropertyValue('height');
    if (image.style.width !== "") {
      width = image.style.width;
    }    
    return { image, width, height };
  }
}
