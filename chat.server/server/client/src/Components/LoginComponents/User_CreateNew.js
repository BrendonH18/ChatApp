import { useParams } from "react-router-dom";

const User_CreateNew = ({ connectedUsers }) => {
  
    let { ActiveChannel } = useParams();

    return(
      <>
      <h2>Create New - {ActiveChannel}</h2>
      </>
    )
  }
  
  export default User_CreateNew;