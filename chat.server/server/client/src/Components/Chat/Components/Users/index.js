import BubbleLabel from "../Formatting/BubbleLabel"
import Card from "../Formatting/Card"

const Users = ({isConnectedUsersByChannelAndStatusTruthy, connectedUsersByChannelAndStatus}) => {
    const Body = () =>{
        return<>
        <ul className="contacts">
                {isConnectedUsersByChannelAndStatusTruthy()
                    ? [{user: {id: 1, username: "ONE"}}, {user: {id: 2, username: "TWO"}}].map(userConnection =>
                        <li id={`contact-${userConnection.user.id}`}>
                            <div className="d-flex bd-highlight" >
                                <div className="user_info" >
                                    <span >{userConnection.user.username}</span>
                                </div>
                            </div>
                        </li>
                    )
                    : <></>}
            </ul>
        
        </>
    }
    
    return <>
    <Card
        header={<BubbleLabel image={"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTC9pw-3QlI-doodCy0D-bsxEBZ9lFTcTFsZQ&usqp=CAU"} label={"Users"} subLabel={`${isConnectedUsersByChannelAndStatusTruthy() ? 3 : 0} Connected`} isOnline={isConnectedUsersByChannelAndStatusTruthy()}/>}
        body={Body()}
        specialFormat={{body: ["contacts_body"]}}
        />
</>
}

export default Users