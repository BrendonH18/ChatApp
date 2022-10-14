import BubbleLabel from "../Formatting/BubbleLabel"

const Users = ({isConnectedUsersByChannelAndStatusTruthy, connectedUsersByChannelAndStatus}) => {
    return <>
    <div className="card mb-sm-3 mb-md-0 contacts_card">
        <div className="card-header">
            <BubbleLabel image={"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTC9pw-3QlI-doodCy0D-bsxEBZ9lFTcTFsZQ&usqp=CAU"} label={"Users"} subLabel={`${isConnectedUsersByChannelAndStatusTruthy() ? 3 : 0} Connected`} isOnline={isConnectedUsersByChannelAndStatusTruthy()}/>
        </div>
        <div className="card-body contacts_body">
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
        </div>
        <div className="card-footer"></div>
    </div>
</>
}

export default Users