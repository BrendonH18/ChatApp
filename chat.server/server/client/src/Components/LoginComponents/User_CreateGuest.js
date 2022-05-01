import { useParams } from "react-router-dom";

const User_CreateGuest = ({ connectedUsers }) => {
  
    let { LoginType } = useParams();

    return(
      <>
      <h2>Create Guest - {LoginType}</h2>
      </>
    )
  }
  
  export default User_CreateGuest;