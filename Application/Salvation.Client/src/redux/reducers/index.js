import { combineReducers } from "redux";
import profileReducer from "./profile";
import gearReducer from "./gear";
import resultsReducer from "./results";

export default combineReducers({ profileReducer, gearReducer, resultsReducer });