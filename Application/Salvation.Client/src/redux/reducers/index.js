import { combineReducers } from "redux";
import profileReducer from "./profile";
import gearReducer from "./gear";

export default combineReducers({ profileReducer, gearReducer });